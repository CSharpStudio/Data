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
    /// <summary>
    /// 实体<see cref="IEntity"/>更新操作接口
    /// </summary>
    public interface IEntityUpdate
    {
        /// <summary>
        /// 表达式
        /// </summary>
        Expression Expression { get; set; }
        /// <summary>
        /// 更新的列值
        /// </summary>
        IList<IColumnValue> Columns { get; }
        /// <summary>
        /// 仓库
        /// </summary>
        IRepository Repository { get; }
        /// <summary>
        /// 执行更新操作
        /// </summary>
        /// <returns></returns>
        int Execute();
    }

    /// <summary>
    /// 实体<see cref="IEntity"/>更新操作接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型参数</typeparam>
    public interface IEntityUpdate<TEntity> : IEntityUpdate
    {
        /// <summary>
        /// Where条件
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEntityUpdate<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IEntityUpdate<TEntity> Set<T>(Expression<Func<TEntity, T>> predicate, T value);
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        IEntityUpdate<TEntity> Set<T>(Expression<Func<TEntity, T>> predicate, Expression<Func<TEntity, T>> expr);
    }
}