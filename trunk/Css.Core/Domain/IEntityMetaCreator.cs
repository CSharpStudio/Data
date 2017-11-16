using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Domain
{
    /// <summary>
    /// 实体<see cref="IEntity"/>元数据<see cref="EntityMeta"/>创建接口
    /// </summary>
    public interface IEntityMetaCreator
    {
        /// <summary>
        /// 创建实体<see cref="IEntity"/>元数据<see cref="EntityMeta"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        EntityMeta Create(Type type);
    }
}
