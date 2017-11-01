using Css.Data;
using Css.Data.Common;
using Css.Data.SqlTree;
using Css.Domain.Query;
using Css.Domain.Query.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{

    class EntityUpdate<TEntity> : IEntityUpdate<TEntity>
    {
        ITableSource _mainTable;
        IRepository _repo;
        public IRepository Repository
        {
            get { return _repo; }
        }
        IList<IColumnValue> _columns;
        public IList<IColumnValue> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// 表达式
        /// </summary>
        public Expression Expression { get; set; }
        SimplePropertyFinder PropertyFinder { get; }
        public EntityUpdate(EntityRepository repo)
        {
            Debug.Assert(repo is IDbRepository, "仓库必须是IDbRepository");
            _repo = repo;
            _mainTable = QueryFactory.Instance.Table(_repo);
            PropertyFinder = new SimplePropertyFinder(_repo);
            _columns = new List<IColumnValue>();
            Expression = Expression.Constant(this);
        }
        public int Execute()
        {
            var expression = Evaluator.PartialEval(Expression);
            var where = new EntityConditionBuilder(_repo).Build(expression);
            var args = new ExecuteArgs(ExecuteType.Update, _mainTable as SqlTable, where as ISqlConstraint, Columns);
            var repo = _repo as IDbRepository;
            using (var dba = repo.CreateDbAccesser())
            {
                return repo.DbTable.Execute(dba, args);
            }
        }

        public IEntityUpdate<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(predicate) };
            Expression = Expression.Call(null, EntityUpdate.GetMethodInfo<IEntityUpdate<TEntity>, Expression<Func<TEntity, bool>>, IEntityUpdate<TEntity>>(new Func<IEntityUpdate<TEntity>, Expression<Func<TEntity, bool>>, IEntityUpdate<TEntity>>(EntityUpdate.Where<TEntity>), this, predicate), arguments);
            return this;
        }

        public IEntityUpdate<TEntity> Set<T>(Expression<Func<TEntity, T>> predicate, T value)
        {
            IProperty property = PropertyFinder.Find(predicate);
            if (property != null)
                Columns.Add(new ColumnValue { PropertyName = property.Name, Value = value });
            return this;
        }

        EntityPropertyFinder _propertyFinder;
        EntityPropertyFinder EntityPropertyFinder
        {
            get { return _propertyFinder ?? (_propertyFinder = new EntityPropertyFinder(QueryFactory.Instance.Query(_repo), _repo, false)); }
        }

        Dictionary<string, ITableSource> _tables;
        Dictionary<string, ITableSource> Tables
        {
            get { return _tables ?? (_tables = new Dictionary<string, ITableSource> { { _mainTable.GetName(), _mainTable } }); }
        }

        public IEntityUpdate<TEntity> Set<T>(Expression<Func<TEntity, T>> predicate, Expression<Func<TEntity, T>> expr)
        {
            IProperty property = PropertyFinder.Find(predicate);
            if (property != null)
            {
                var expression = Evaluator.PartialEval(expr.Body);
                var visitor = new SelectionVisitor(Tables, EntityPropertyFinder);
                visitor.Visit(expression);
                var value = visitor.Columns.FirstOrDefault();
                Columns.Add(new ColumnValue { PropertyName = property.Name, Value = value });
            }
            return this;
        }
    }

    internal static class EntityUpdate
    {
        public static IEntityUpdate<TEntity> Where<TEntity>(this IEntityUpdate<TEntity> source, Expression<Func<TEntity, bool>> predicate)
        {
            Expression[] arguments = new Expression[] { source.Expression, Expression.Quote(predicate) };

            source.Expression = Expression.Call(null, GetMethodInfo<IEntityUpdate<TEntity>, Expression<Func<TEntity, bool>>, IEntityUpdate<TEntity>>(new Func<IEntityUpdate<TEntity>, Expression<Func<TEntity, bool>>, IEntityUpdate<TEntity>>(EntityUpdate.Where<TEntity>), source, predicate), arguments);

            return source;
        }

        public static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 unused1, T2 unused2)
        {
            return f.Method;
        }

        public static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> f, T1 unused1)
        {
            return f.Method;
        }
    }
}