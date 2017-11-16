using System;
using System.Linq.Expressions;

namespace Css.Domain
{
    /// <summary>
    /// 实体<see cref="IEntity"/>删除操作接口
    /// </summary>
    public interface IEntityDelete
    {
        /// <summary>
        /// 表达式
        /// </summary>
        Expression Expression { get; set; }
        /// <summary>
        /// 仓库
        /// </summary>
        IRepository Repository { get; }
        /// <summary>
        /// 执行删除操作
        /// </summary>
        /// <returns></returns>
        int Execute();
    }

    /// <summary>
    /// 实体<see cref="IEntity"/>删除操作接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型参数</typeparam>
    public interface IEntityDelete<TEntity> : IEntityDelete
    {
        /// <summary>
        /// Where条件
        /// </summary>
        /// <param name="predicate">表达式</param>
        /// <returns></returns>
        IEntityDelete<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
    }
}
