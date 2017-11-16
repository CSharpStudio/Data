using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 实体<see cref="IEntity"/>配置接口，实体元数据<see cref="EntityMeta"/>的配置
    /// </summary>
    public interface IEntityConfig
    {
        /// <summary>
        /// 实体元数据<see cref="EntityMeta"/>
        /// </summary>
        EntityMeta Meta { get; set; }
        /// <summary>
        /// 配置元数据
        /// </summary>
        void ConfigMeta();
        /// <summary>
        /// 实体<see cref="IEntity"/>类型
        /// </summary>
        Type EntityType { get; }
    }
}
