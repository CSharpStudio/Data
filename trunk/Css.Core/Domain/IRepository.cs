using Css.Data;
using Css.Domain.Query;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Css.Domain
{
    /// <summary>
    /// 仓库接口
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }
        /// <summary>
        /// 实体元数据
        /// </summary>
        EntityMeta EntityMeta { get; }
        /// <summary>
        /// 根据ID查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns>实体，如果不存在返回null</returns>
        IEntity GetById(object id);
        /// <summary>
        /// 保存领域对象
        /// </summary>
        /// <param name="entity"></param>
        void Save(IDomain entity);
        /// <summary>
        /// 计数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        int Count(IQuery query);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        IList ToList(IQuery query, int start, int end);
        /// <summary>
        /// 查询满足条件的第一个实体
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEntity FirstOrDefault(IQuery query);
    }
}
