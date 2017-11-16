using Css.Data.Filtering;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Css.Domain
{
    /// <summary>
    /// 实体<see cref="IEntity"/>查询操作接口
    /// </summary>
    public interface IEntityQueryer
    {
        /// <summary>
        /// 表达式
        /// </summary>
        Expression Expression { get; set; }
        /// <summary>
        /// 主表仓库
        /// </summary>
        IRepository Repository { get; }
        /// <summary>
        /// 主表别名
        /// </summary>
        string Alias { get; set; }
        /// <summary>
        /// 分页开始数
        /// </summary>
        int Start { get; }
        /// <summary>
        /// 分页结束数
        /// </summary>
        int End { get; }
        /// <summary>
        /// Where标准条件
        /// </summary>
        CriteriaOperator WhereCriteria { get; }
        /// <summary>
        /// Having标准条件
        /// </summary>
        CriteriaOperator HavingCriteria { get; }
    }

    /// <summary>
    /// 实体查询
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IEntityQueryer<TEntity> : IEntityQueryer
    {
        /// <summary>
        /// 主表别名
        /// </summary>
        IEntityQueryer<TEntity> As(string alias);
        /// <summary>
        /// 选择返回结果
        /// </summary>
        IEntityQueryer<TEntity> Select(Expression<Func<TEntity, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1>(Expression<Func<TEntity, T1, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2>(Expression<Func<TEntity, T1, T2, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> selector);
        /// <summary>
        /// 选择返回结果,需要写在join后,否则找不到关联的实体属性
        /// </summary>
        IEntityQueryer<TEntity> Select<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> selector);

        IEntityQueryer<TEntity> Where(Expression<Func<TEntity, bool>> expr);
        IEntityQueryer<TEntity> Where<T1>(Expression<Func<TEntity, T1, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2>(Expression<Func<TEntity, T1, T2, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> expr);
        IEntityQueryer<TEntity> Where<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> expr);

        IEntityQueryer<TEntity> Having(Expression<Func<TEntity, bool>> expr);
        IEntityQueryer<TEntity> Having<T1>(Expression<Func<TEntity, T1, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2>(Expression<Func<TEntity, T1, T2, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, bool>> expr);
        IEntityQueryer<TEntity> Having<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, bool>> expr);

        /// <summary>
        /// 实体关联查询
        /// </summary>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> Join<TRight>(Expression<Func<TEntity, TRight, bool>> expr);
        /// <summary>
        /// 实体关联查询
        /// </summary>
        /// <typeparam name="TLeft">必须是之前关联过的类型</typeparam>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> Join<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expr);
        /// <summary>
        /// 实体左关联查询
        /// </summary>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> LeftJoin<TRight>(Expression<Func<TEntity, TRight, bool>> expr);
        /// <summary>
        /// 实体左关联查询
        /// </summary>
        /// <typeparam name="TLeft">必须是之前关联过的类型</typeparam>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> LeftJoin<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expr);
        /// <summary>
        /// 实体右关联查询
        /// </summary>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> RightJoin<TRight>(Expression<Func<TEntity, TRight, bool>> expr);
        /// <summary>
        /// 实体右关联查询
        /// </summary>
        /// <typeparam name="TLeft">必须是之前关联过的类型</typeparam>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> RightJoin<TLeft, TRight>(Expression<Func<TLeft, TRight, bool>> expr);

        /// <summary>
        /// 实体关联查询
        /// </summary>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> Join<TRight>(string alias, Expression<Func<TEntity, TRight, bool>> expr);
        /// <summary>
        /// 实体关联查询
        /// </summary>
        /// <typeparam name="TLeft">必须是之前关联过的类型</typeparam>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> Join<TLeft, TRight>(string alias, Expression<Func<TLeft, TRight, bool>> expr);
        /// <summary>
        /// 实体左关联查询
        /// </summary>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> LeftJoin<TRight>(string alias, Expression<Func<TEntity, TRight, bool>> expr);
        /// <summary>
        /// 实体左关联查询
        /// </summary>
        /// <typeparam name="TLeft">必须是之前关联过的类型</typeparam>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> LeftJoin<TLeft, TRight>(string alias, Expression<Func<TLeft, TRight, bool>> expr);
        /// <summary>
        /// 实体右关联查询
        /// </summary>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> RightJoin<TRight>(string alias, Expression<Func<TEntity, TRight, bool>> expr);
        /// <summary>
        /// 实体右关联查询
        /// </summary>
        /// <typeparam name="TLeft">必须是之前关联过的类型</typeparam>
        /// <typeparam name="TRight">新关联的类型</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="expr">关联条件</param>
        /// <returns></returns>
        IEntityQueryer<TEntity> RightJoin<TLeft, TRight>(string alias, Expression<Func<TLeft, TRight, bool>> expr);

        IEntityQueryer<TEntity> OrderBy(Expression<Func<TEntity, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1>(Expression<Func<TEntity, T1, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2>(Expression<Func<TEntity, T1, T2, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr);
        IEntityQueryer<TEntity> OrderBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr);

        IEntityQueryer<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1>(Expression<Func<TEntity, T1, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2>(Expression<Func<TEntity, T1, T2, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr);
        IEntityQueryer<TEntity> OrderByDescending<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr);

        IEntityQueryer<TEntity> GroupBy(Expression<Func<TEntity, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1>(Expression<Func<TEntity, T1, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2>(Expression<Func<TEntity, T1, T2, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3>(Expression<Func<TEntity, T1, T2, T3, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4>(Expression<Func<TEntity, T1, T2, T3, T4, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5>(Expression<Func<TEntity, T1, T2, T3, T4, T5, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, object>> expr);
        IEntityQueryer<TEntity> GroupBy<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, object>> expr);

        IEntityQueryer<TEntity> Distinct();
        IEntityQueryer<TEntity> Skip(int count);
        IEntityQueryer<TEntity> Take(int count);
        IEntityQueryer<TEntity> Where(string criteria);
        IEntityQueryer<TEntity> Where(CriteriaOperator criteria);
        IEntityQueryer<TEntity> Having(string criteria);
        IEntityQueryer<TEntity> Having(CriteriaOperator criteria);

        /// <summary>
        /// Exists查询
        /// </summary>
        IEntityQueryer<TEntity> Exists<T>(Expression<Func<TEntity, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T>(Expression<Func<TEntity, T1, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T>(Expression<Func<TEntity, T1, T2, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T>(Expression<Func<TEntity, T1, T2, T3, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T>(Expression<Func<TEntity, T1, T2, T3, T4, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> Exists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IEntityQueryer<T>, IEntityQueryer<T>>> expr);

        /// <summary>
        /// Exists查询
        /// </summary>
        IEntityQueryer<TEntity> NotExists<T>(Expression<Func<TEntity, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T>(Expression<Func<TEntity, T1, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T>(Expression<Func<TEntity, T1, T2, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T>(Expression<Func<TEntity, T1, T2, T3, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T>(Expression<Func<TEntity, T1, T2, T3, T4, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
        IEntityQueryer<TEntity> NotExists<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T>(Expression<Func<TEntity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, IEntityQueryer<T>, IEntityQueryer<T>>> expr);
    }
}

