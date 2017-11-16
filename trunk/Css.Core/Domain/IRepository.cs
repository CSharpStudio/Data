using Css.Data;
using Css.Domain.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 仓库接口，提供实体的CURD基本操作，包括实体的元数据信息<see cref="Domain.EntityMeta"/>
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 获取实体类型
        /// </summary>
        Type EntityType { get; }
        /// <summary>
        /// 获取实体元数据
        /// </summary>
        EntityMeta EntityMeta { get; }
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns>实体，如果不存在返回null</returns>
        IEntity GetById(object id);
        /// <summary>
        /// 根据ID异步获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEntity> GetByIdAsync(object id);
        /// <summary>
        /// 保存领域对象
        /// </summary>
        /// <param name="entity"></param>
        void Save(IDomain entity);
        /// <summary>
        /// 异步保存领域对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task SaveAsync(IDomain entity);
        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        int Count(IQuery query);
        /// <summary>
        /// 异步计数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<int> CountAsync(IQuery query);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IEntityList ToList(IQuery query, int start, int end);
        /// <summary>
        /// 异步获取列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<IEntityList> ToListAsync(IQuery query, int start, int end);
        /// <summary>
        /// 查询满足条件的第一个实体
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEntity FirstOrDefault(IQuery query);
        /// <summary>
        /// 异步查询满足条件的第一个实体
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IEntity> FirstOrDefaultAsync(IQuery query);
    }
}
