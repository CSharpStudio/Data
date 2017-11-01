using Css.Data;
using Css.Domain.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public interface IEntityUpdate
    {
        Expression Expression { get; set; }
        IList<IColumnValue> Columns { get; }
        IRepository Repository { get; }
        int Execute();
    }

    public interface IEntityUpdate<TEntity> : IEntityUpdate
    {
        IEntityUpdate<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IEntityUpdate<TEntity> Set<T>(Expression<Func<TEntity, T>> predicate, T value);
        IEntityUpdate<TEntity> Set<T>(Expression<Func<TEntity, T>> predicate, Expression<Func<TEntity, T>> expr);
    }
}