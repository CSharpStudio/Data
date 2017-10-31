using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    /// <summary>
    /// 链式查询条件拼装接口。
    /// 
    /// 简单封装了 QueryFactory 类型。
    /// </summary>
    public static partial class QueryExtensions
    {
        /// <summary>
        /// 如果提供的值是不可空的，则为查询添加一个对应的约束条件，并以 And 与原条件进行连接。
        /// </summary>
        /// <param name="query">查询.</param>
        /// <param name="property">要约束的属性.</param>
        /// <param name="value">当 value 不可空时，才添加这个对比约束条件。</param>
        /// <returns></returns>
        public static IQuery AddConstraintIf(this IQuery query, IProperty property, object value)
        {
            return AddConstraintIf(query, property, BinaryOp.Equal, value);
        }

        /// <summary>
        /// 如果提供的值是不可空的，则为查询添加一个对应的约束条件，并以 And 与原条件进行连接。
        /// </summary>
        /// <param name="query">查询.</param>
        /// <param name="property">要约束的属性.</param>
        /// <param name="op">约束条件操作符.</param>
        /// <param name="value">当 value 不可空时，才添加这个对比约束条件。</param>
        /// <returns></returns>
        public static IQuery AddConstraintIf(this IQuery query, IProperty property, BinaryOp op, object value)
        {
            if (value != null || (value is string && !string.IsNullOrEmpty(value?.ToString())))
            {
                return AddConstraint(query, property, op, value);
            }
            return query;
        }

        /// <summary>
        /// 如果提供的值是不可空的，则为查询添加一个对应的约束条件，并以 And 与原条件进行连接。
        /// </summary>
        /// <param name="query">查询.</param>
        /// <param name="property">要约束的属性.</param>
        /// <param name="op">约束条件操作符.</param>
        /// <param name="value">当 value 不可空时，才添加这个对比约束条件。</param>
        /// <param name="propertySource">指定该属性所属的实体数据源。</param>
        /// <returns></returns>
        public static IQuery AddConstraintIf(this IQuery query, IProperty property, BinaryOp op, object value, ITableSource propertySource)
        {
            if (value != null || (value is string && !string.IsNullOrEmpty(value?.ToString())))
            {
                return AddConstraint(query, property, op, value, propertySource);
            }

            return query;
        }

        /// <summary>
        /// 为查询添加一个对应的约束条件，并以 And 与原条件进行连接。
        /// </summary>
        /// <param name="query">查询.</param>
        /// <param name="property">要约束的属性.</param>
        /// <param name="value">对比的值。</param>
        /// <returns></returns>
        public static IQuery AddConstraint(this IQuery query, IProperty property, object value)
        {
            return AddConstraint(query, property, BinaryOp.Equal, value);
        }

        /// <summary>
        /// 为查询添加一个对应的约束条件，并以 And 与原条件进行连接。
        /// </summary>
        /// <param name="query">查询.</param>
        /// <param name="property">要约束的属性.</param>
        /// <param name="op">约束条件操作符.</param>
        /// <param name="value">对比的值。</param>
        /// <returns></returns>
        public static IQuery AddConstraint(this IQuery query, IProperty property, BinaryOp op, object value)
        {
            var source = query.MainTable;
            if (!property.OwnerType.IsAssignableFrom(source.EntityRepository.EntityType))
            {
                source = query.From.FindTable(RF.Find(property.OwnerType));
            }
            return AddConstraint(query, property, op, value, source);
        }

        /// <summary>
        /// 为查询添加一个对应的约束条件，并以 And 与原条件进行连接。
        /// </summary>
        /// <param name="query">查询.</param>
        /// <param name="property">要约束的属性.</param>
        /// <param name="op">约束条件操作符.</param>
        /// <param name="value">对比的值。</param>
        /// <param name="propertySource">指定该属性所属的实体数据源。</param>
        /// <returns></returns>
        public static IQuery AddConstraint(this IQuery query, IProperty property, BinaryOp op, object value, ITableSource propertySource)
        {
            var f = QueryFactory.Instance;

            var propertyNode = propertySource.Column(property.Name);

            var where = f.Constraint(propertyNode, op, value);
            if (query.Where == null)
            {
                query.Where = where;
            }
            else
            {
                query.Where = f.And(query.Where, where);
            }

            return query;
        }

        public static ISelectAll Star(this ITableSource table)
        {
            return QueryFactory.Instance.SelectAll(table);
        }

        public static IJoin Join(this ITableSource left, ITableSource right)
        {
            return QueryFactory.Instance.Join(left, right);
        }
        public static IJoin Join(this ISource left, ITableSource right, IRefProperty leftToRight)
        {
            return QueryFactory.Instance.Join(left, right, leftToRight);
        }
        public static IJoin Join(this ISource left, ITableSource right, IConstraint condition, JoinType joinType = JoinType.Inner)
        {
            return QueryFactory.Instance.Join(left, right, condition, joinType);
        }

        public static IConstraint Equal(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Equal, value);
        }
        public static IConstraint NotEqual(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotEqual, value);
        }
        public static IConstraint Greater(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Greater, value);
        }
        public static IConstraint GreaterEqual(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.GreaterEqual, value);
        }
        public static IConstraint Less(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Less, value);
        }
        public static IConstraint LessEqual(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.LessEqual, value);
        }
        public static IConstraint Like(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Like, value);
        }
        public static IConstraint NotLike(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotLike, value);
        }
        public static IConstraint Contains(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Contains, value);
        }
        public static IConstraint NotContains(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotContains, value);
        }
        public static IConstraint StartWith(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.StartsWith, value);
        }
        public static IConstraint NotStartWith(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotStartsWith, value);
        }
        public static IConstraint EndWith(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.EndsWith, value);
        }
        public static IConstraint NotEndWith(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotEndsWith, value);
        }
        public static IConstraint In(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.In, value);
        }
        public static IConstraint NotIn(this IColumnNode column, object value)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotIn, value);
        }

        public static IConstraint Equal(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Equal, rightColumn);
        }
        public static IConstraint NotEqual(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotEqual, rightColumn);
        }
        public static IConstraint Greater(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Greater, rightColumn);
        }
        public static IConstraint GreaterEqual(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.GreaterEqual, rightColumn);
        }
        public static IConstraint Less(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Less, rightColumn);
        }
        public static IConstraint LessEqual(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.LessEqual, rightColumn);
        }
        public static IConstraint Like(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Like, rightColumn);
        }
        public static IConstraint NotLike(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotLike, rightColumn);
        }
        public static IConstraint Contains(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.Contains, rightColumn);
        }
        public static IConstraint NotContains(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotContains, rightColumn);
        }
        public static IConstraint StartWith(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.StartsWith, rightColumn);
        }
        public static IConstraint NotStartWith(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotStartsWith, rightColumn);
        }
        public static IConstraint EndWith(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.EndsWith, rightColumn);
        }
        public static IConstraint NotEndWith(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotEndsWith, rightColumn);
        }
        public static IConstraint In(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.In, rightColumn);
        }
        public static IConstraint NotIn(this IColumnNode column, IColumnNode rightColumn)
        {
            return QueryFactory.Instance.Constraint(column, BinaryOp.NotIn, rightColumn);
        }
    }
}