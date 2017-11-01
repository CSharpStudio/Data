using Css.Data.Filtering;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Css.Domain
{

    class EntityQueryer<TEntity> : IEntityQueryer<TEntity>
    {
        CriteriaOperator _whereCriteria;
        CriteriaOperator _havingCriteria;
        int _skip;
        int _take;

        public Expression Expression { get; set; }

        public IRepository Repository { get; }

        public string Alias { get; set; }

        public int Start { get { return _skip; } }

        public int End { get { return _skip + _take; } }

        public CriteriaOperator WhereCriteria { get { return _whereCriteria; } }
        public CriteriaOperator HavingCriteria { get { return _havingCriteria; } }

        public IEntityQueryer<TEntity> Where(string criteria)
        {
            if (object.ReferenceEquals(_whereCriteria, null))
                _whereCriteria = CriteriaOperator.Parse(criteria);
            else
                CriteriaOperator.And(_whereCriteria, CriteriaOperator.Parse(criteria));
            return this;
        }

        public IEntityQueryer<TEntity> Where(CriteriaOperator criteria)
        {
            if (object.ReferenceEquals(_whereCriteria, null))
                _whereCriteria = criteria;
            else
                CriteriaOperator.And(_whereCriteria, criteria);
            return this;
        }
        public IEntityQueryer<TEntity> Having(string criteria)
        {
            if (object.ReferenceEquals(_havingCriteria, null))
                _havingCriteria = CriteriaOperator.Parse(criteria);
            else
                CriteriaOperator.And(_havingCriteria, CriteriaOperator.Parse(criteria));
            return this;
        }

        public IEntityQueryer<TEntity> Having(CriteriaOperator criteria)
        {
            if (object.ReferenceEquals(_havingCriteria, null))
                _havingCriteria = criteria;
            else
                CriteriaOperator.And(_havingCriteria, criteria);
            return this;
        }

        public IEntityQueryer<TEntity> Skip(int skip)
        {
            if (skip < 0)
                throw new ArgumentOutOfRangeException(nameof(skip), "不能小于0");
            _skip = skip;
            return this;
        }

        public IEntityQueryer<TEntity> Take(int take)
        {
            if (take < 0)
                throw new ArgumentOutOfRangeException(nameof(take), "不能小于0");
            _take = take;
            return this;
        }

        public EntityQueryer(IRepository repo, string alias = null)
        {
            Repository = repo;
            Expression = Expression.Constant(this);
            Alias = alias;
        }

        public IEntityQueryer<TEntity> As(string alias)
        {
            if (alias == null)
                throw new ArgumentNullException(nameof(alias));
            if (Alias.IsNotEmpty())
                throw new ORMException("已经存在别名{0}".FormatArgs(Alias));
            Alias = alias;
            return this;
        }

        #region Select

        public IEntityQueryer<TEntity> Select(Expression<Func<TEntity, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1>(Expression<Func<TEntity, T1, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2>(Expression<Func<TEntity, T1, T2, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Select, this, expr), arguments);
            return this;
        }

        #endregion

        #region Exists

        public IEntityQueryer<TEntity> Exists<T>(Expression<Func<TEntity, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T>(Expression<Func<TEntity, T1, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T>(Expression<Func<TEntity, T1, T2, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T>(Expression<Func<TEntity, T1, T2, T3, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T>(Expression<Func<TEntity, T1, T2, T3, T4, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Exists, this, expr), arguments);
            return this;
        }

        #endregion

        #region NotExists

        public IEntityQueryer<TEntity> NotExists<T>(Expression<Func<TEntity, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T>(Expression<Func<TEntity, T1, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T>(Expression<Func<TEntity, T1, T2, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T>(Expression<Func<TEntity, T1, T2, T3, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T>(Expression<Func<TEntity, T1, T2, T3, T4, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IEntityQueryer<T>, IEntityQueryer<T>>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.NotExists, this, expr), arguments);
            return this;
        }

        #endregion

        #region Where

        public IEntityQueryer<TEntity> Where(Expression<Func<TEntity, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1>(Expression<Func<TEntity, T1, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2>(Expression<Func<TEntity, T1, T2, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Where, this, expr), arguments);
            return this;
        }

        #endregion

        #region Having

        public IEntityQueryer<TEntity> Having(Expression<Func<TEntity, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1>(Expression<Func<TEntity, T1, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2>(Expression<Func<TEntity, T1, T2, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Having, this, expr), arguments);
            return this;
        }

        #endregion

        #region Join

        public IEntityQueryer<TEntity> Join<TRight>(Expression<Func<TEntity, TRight, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Join<TEntity, TRight>, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Join<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Join, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> LeftJoin<TRight>(Expression<Func<TEntity, TRight, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.LeftJoin<TEntity, TRight>, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> LeftJoin<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.LeftJoin, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> RightJoin<TRight>(Expression<Func<TEntity, TRight, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.RightJoin<TEntity, TRight>, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> RightJoin<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.RightJoin, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Join<TRight>(string alias, Expression<Func<TEntity, TRight, bool>> expr)
        {
            if (alias.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(alias));
            Expression[] arguments = new Expression[] { Expression, Expression.Constant(alias), Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Join<TEntity, TRight>, this, alias, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> Join<TLeft, TRight>(string alias, Expression<Func<TLeft, TRight, bool>> expr)
        {
            if (alias.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(alias));
            Expression[] arguments = new Expression[] { Expression, Expression.Constant(alias), Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Join, this, alias, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> LeftJoin<TRight>(string alias, Expression<Func<TEntity, TRight, bool>> expr)
        {
            if (alias.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(alias));
            Expression[] arguments = new Expression[] { Expression, Expression.Constant(alias), Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.LeftJoin<TEntity, TRight>, this, alias, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> LeftJoin<TLeft, TRight>(string alias, Expression<Func<TLeft, TRight, bool>> expr)
        {
            if (alias.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(alias));
            Expression[] arguments = new Expression[] { Expression, Expression.Constant(alias), Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.LeftJoin, this, alias, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> RightJoin<TRight>(string alias, Expression<Func<TEntity, TRight, bool>> expr)
        {
            if (alias.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(alias));
            Expression[] arguments = new Expression[] { Expression, Expression.Constant(alias), Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.RightJoin<TEntity, TRight>, this, alias, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> RightJoin<TLeft, TRight>(string alias, Expression<Func<TLeft, TRight, bool>> expr)
        {
            if (alias.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(alias));
            Expression[] arguments = new Expression[] { Expression, Expression.Constant(alias), Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.RightJoin, this, alias, expr), arguments);
            return this;
        }

        #endregion

        #region OrderBy

        public IEntityQueryer<TEntity> OrderBy(Expression<Func<TEntity, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1>(Expression<Func<TEntity, T1, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2>(Expression<Func<TEntity, T1, T2, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1>(Expression<Func<TEntity, T1, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2>(Expression<Func<TEntity, T1, T2, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.OrderByDescending, this, expr), arguments);
            return this;
        }

        #endregion

        #region

        public IEntityQueryer<TEntity> GroupBy(Expression<Func<TEntity, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1>(Expression<Func<TEntity, T1, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2>(Expression<Func<TEntity, T1, T2, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        public IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            Expression[] arguments = new Expression[] { Expression, Expression.Quote(expr) };
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.GroupBy, this, expr), arguments);
            return this;
        }

        #endregion

        public IEntityQueryer<TEntity> Distinct()
        {
            Expression = Expression.Call(null, EntityQueryer.GetMethodInfo(EntityQueryer.Distinct, this), new Expression[] { Expression });
            return this;
        }
    }

    static class EntityQueryer
    {
        static NotSupportedException Error()
        {
            return new NotSupportedException("Not to be used directly - use inside IEntityQueryer expression");
        }

        #region Select

        public static IEntityQueryer<TEntity> Select<TEntity>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Select<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            throw Error();
        }

        #endregion

        #region Where

        public static IEntityQueryer<TEntity> Where<TEntity>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Where<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> expr)
        {
            throw Error();
        }

        #endregion

        #region Having

        public static IEntityQueryer<TEntity> Having<TEntity>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Having<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> expr)
        {
            throw Error();
        }

        #endregion

        #region Join

        public static IEntityQueryer<TSource> Join<TSource, TRight>(this IEntityQueryer<TSource> source, Expression<Func<TSource, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> Join<TSource, TLeft, TRight>(this IEntityQueryer<TSource> source, Expression<Func<TLeft, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> LeftJoin<TSource, TRight>(this IEntityQueryer<TSource> source, Expression<Func<TSource, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> LeftJoin<TSource, TLeft, TRight>(this IEntityQueryer<TSource> source, Expression<Func<TLeft, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> RightJoin<TSource, TRight>(this IEntityQueryer<TSource> source, Expression<Func<TSource, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> RightJoin<TSource, TLeft, TRight>(this IEntityQueryer<TSource> source, Expression<Func<TLeft, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> Join<TSource, TRight>(this IEntityQueryer<TSource> source, string alias, Expression<Func<TSource, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> Join<TSource, TLeft, TRight>(this IEntityQueryer<TSource> source, string alias, Expression<Func<TLeft, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> LeftJoin<TSource, TRight>(this IEntityQueryer<TSource> source, string alias, Expression<Func<TSource, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> LeftJoin<TSource, TLeft, TRight>(this IEntityQueryer<TSource> source, string alias, Expression<Func<TLeft, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> RightJoin<TSource, TRight>(this IEntityQueryer<TSource> source, string alias, Expression<Func<TSource, TRight, bool>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> RightJoin<TSource, TLeft, TRight>(this IEntityQueryer<TSource> source, string alias, Expression<Func<TLeft, TRight, bool>> expr)
        {
            throw Error();
        }

        #endregion

        #region OrderBy

        public static IEntityQueryer<TSource> OrderBy<TSource>(this IEntityQueryer<TSource> source, Expression<Func<TSource, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderBy<TSource, T1>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderBy<TSource, T1, T2>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderBy<TSource, T1, T2, T3>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderBy<TSource, T1, T2, T3, T4>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, T4, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderBy<TSource, T1, T2, T3, T4, T5>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, T4, T5, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderByDescending<TSource>(this IEntityQueryer<TSource> source, Expression<Func<TSource, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderByDescending<TSource, T1>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderByDescending<TSource, T1, T2>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderByDescending<TSource, T1, T2, T3>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderByDescending<TSource, T1, T2, T3, T4>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, T4, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> OrderByDescending<TSource, T1, T2, T3, T4, T5>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, T4, T5, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> OrderByDescending<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            throw Error();
        }

        #endregion

        #region GroupBy

        public static IEntityQueryer<TSource> GroupBy<TSource>(this IEntityQueryer<TSource> source, Expression<Func<TSource, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> GroupBy<TSource, T1>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> GroupBy<TSource, T1, T2>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> GroupBy<TSource, T1, T2, T3>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> GroupBy<TSource, T1, T2, T3, T4>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, T4, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TSource> GroupBy<TSource, T1, T2, T3, T4, T5>(this IEntityQueryer<TSource> source, Expression<Func<TSource, T1, T2, T3, T4, T5, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> GroupBy<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr)
        {
            throw Error();
        }

        #endregion

        #region Exists

        public static IEntityQueryer<TEntity> Exists<TEntity, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> Exists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        #endregion

        #region NotExists

        public static IEntityQueryer<TEntity> NotExists<TEntity, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        public static IEntityQueryer<TEntity> NotExists<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TExists>(this IEntityQueryer<TEntity> source, Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IEntityQueryer<TExists>, IEntityQueryer<TExists>>> expr)
        {
            throw Error();
        }

        #endregion

        public static IEntityQueryer<TSource> Distinct<TSource>(this IEntityQueryer<TSource> source)
        {
            throw Error();
        }

        #region GetMethodInfo

        public static MethodInfo GetMethodInfo<T1, T2, T3, T4>(Func<T1, T2, T3, T4> f, T1 t1, T2 t2, T3 t3)
        {
            return f.Method;
        }

        public static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> f, T1 t1, T2 t2)
        {
            return f.Method;
        }

        public static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> f, T1 t1)
        {
            return f.Method;
        }

        #endregion
    }
}

