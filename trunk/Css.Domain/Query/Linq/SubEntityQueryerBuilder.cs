﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Linq
{
    class SubEntityQueryerBuilder : ExpressionVisitor
    {
        private QueryFactory f = QueryFactory.Instance;
        private IQuery _parentQuery;
        private EntityPropertyFinder _parentPropertyFinder;
        private IQuery _query;
        private bool _isAny;
        Dictionary<string, ITableSource> _tables;

        /// <summary>
        /// <![CDATA[
        /// 将聚合子表达式解析为嵌入式子查询条件。
        /// 例如：
        /// 将表达式
        /// book.ChapterList.Cast<Chapter>().Any(c => c.Name == chapterName)
        /// 转换为：
        /// f.Exists(f.Query(chapter,
        ///     where: f.And(
        ///         f.Constraint(chapter.Column(Chapter.BookIdProperty), book.IdColumn),
        ///         f.Constraint(chapter.Column(Chapter.NameProperty), chapterName)
        ///     )
        /// ))
        /// SQL：
        /// SELECT * FROM [Book] b
        /// WHERE EXISTS(
        ///     SELECT * FROM [Chapter] c
        ///     WHERE c.BookId = b.Id AND
        ///         c.Name = {0}
        /// )
        /// ]]>
        /// </summary>
        /// <param name="exp">需要解析的表达式</param>
        /// <param name="parentQuery"></param>
        /// <param name="tables"></param>
        /// <param name="propertyFinder">The property finder.</param>
        internal IConstraint Build(Expression exp, IQuery parentQuery, Dictionary<string, ITableSource> tables, EntityPropertyFinder propertyFinder)
        {
            _tables = tables;
            _parentQuery = parentQuery;
            _parentPropertyFinder = propertyFinder;

            Visit(exp);

            var res = f.Exists(_query);
            if (!_isAny) res = f.Not(res);

            //如果父查询中需要反转条件，则返回 NOT 语句。
            if (propertyFinder.ReverseConstraint)
            {
                res = f.Not(res);
            }

            //把可空外键的不可空条件，与 Exists 条件合并后返回。
            if (propertyFinder.NullableRefConstraint != null)
            {
                var op = propertyFinder.ReverseConstraint ? GroupOp.Or : GroupOp.And;
                res = f.Binary(propertyFinder.NullableRefConstraint, op, res);
            }

            return res;
        }

        protected override Expression VisitMethodCall(MethodCallExpression exp)
        {
            var method = exp.Method;
            _isAny = method.Name == LinqMethods.Any;

            //先访问表达式 book.ChapterList.Cast<Chapter>()，获取列表属性。
            var invoker = exp.Arguments[0];
            _parentPropertyFinder.Find(invoker, _tables);
            var listPropertyTable = _parentPropertyFinder.PropertyOwnerTable;
            var listProperty = _parentPropertyFinder.Property as IListProperty;
            if (listProperty == null) throw EntityQueryerBuilder.OperationNotSupported(invoker);

            //为该列表对应的实体创建表对象、查询对象。
            var childRepo = RF.Find(listProperty.PropertyType.GetGenericType(typeof(EntityList<>).GetGenericArguments()[0]));
            var childTable = f.Table(childRepo);
            _query = f.Query(
                from: childTable,
                selection: f.Literal("1")
                );
            var qgc = QueryGenerationContext.Get(_parentQuery);
            qgc.Bind(_query);
            childTable.Alias = qgc.NextTableAlias();

            //Any、All 方法如果有第二个参数，那么第二个参数就是条件表达式。如：c => c.Name == chapterName
            if (exp.Arguments.Count == 2)
            {
                var reverseWhere = !_isAny;//如果是 All，则需要反转里面的所有操作符。
                var queryBuilder = new EntityQueryerBuilder(childRepo, reverseWhere);
                queryBuilder.BuildQuery(exp.Arguments[1], _query);
            }

            //添加子表查询与父实体的关系条件：WHERE c.BookId = b.Id
            var parentProperty = (childRepo as IRepository).GetParentProperty();
            var parentRefIdProperty = parentProperty.RefIdProperty;
            var toParentConstraint = f.Constraint(childTable.Column(parentRefIdProperty.Name), listPropertyTable.IdColumn);
            _query.Where = f.And(toParentConstraint, _query.Where);

            return exp;
        }
    }
}