using System;
using System.Linq.Expressions;

namespace Css.Domain
{
    public interface IEntityDelete
    {
        Expression Expression { get; set; }
        IRepository Repository { get; }
        int Execute();
    }

    public interface IEntityDelete<TEntity> : IEntityDelete
    {
        IEntityDelete<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
    }
}
