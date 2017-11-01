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
    class EntityDelete<TEntity> : IEntityDelete<TEntity>
    {
        IRepository _repo;
        public IRepository Repository
        {
            get { return _repo; }
        }
        SimplePropertyFinder PropertyFinder { get; }
        public EntityDelete(IRepository repo)
        {
            Debug.Assert(repo is IDbRepository, "仓库必须是IDbRepository");
            _repo = repo;
            PropertyFinder = new SimplePropertyFinder(_repo);
            Expression = Expression.Constant(this);
        }
        public int Execute()
        {
            var mainTable = QueryFactory.Instance.Table(_repo);
            var expression = Evaluator.PartialEval(Expression);
            var where = new EntityConditionBuilder(_repo).Build(expression);
            var args = new ExecuteArgs(ExecuteType.Delete, mainTable as SqlTable, where as ISqlConstraint);
            var repo = _repo as IDbRepository;
            using (var dba = repo.CreateDbAccesser())
            {
                return repo.DbTable.Execute(dba, args);
            }
        }

        /// <summary>
        /// 表达式
        /// </summary>
        public Expression Expression { get; set; }

        public IEntityDelete<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(predicate) };
            Expression = Expression.Call(null, EntityDelete.GetMethodInfo<IEntityDelete<TEntity>, Expression<Func<TEntity, bool>>, IEntityDelete<TEntity>>(new Func<IEntityDelete<TEntity>, Expression<Func<TEntity, bool>>, IEntityDelete<TEntity>>(EntityDelete.Where<TEntity>), this, predicate), arguments);
            return this;
        }
    }

    internal static class EntityDelete
    {
        public static IEntityDelete<TEntity> Where<TEntity>(this IEntityDelete<TEntity> source, Expression<Func<TEntity, bool>> predicate)
        {
            Expression[] arguments = new Expression[] { source.Expression, Expression.Quote(predicate) };

            source.Expression = Expression.Call(null, GetMethodInfo<IEntityDelete<TEntity>, Expression<Func<TEntity, bool>>, IEntityDelete<TEntity>>(new Func<IEntityDelete<TEntity>, Expression<Func<TEntity, bool>>, IEntityDelete<TEntity>>(EntityDelete.Where<TEntity>), source, predicate), arguments);

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
