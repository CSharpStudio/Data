using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 实体元数据事件参数
    /// </summary>
    public class EntityMetaEventArgs : EventArgs
    {
        /// <summary>
        /// 元数据
        /// </summary>
        public EntityMeta Meta { get; private set; }

        /// <summary>
        /// 构造实体元数据事件参数
        /// </summary>
        /// <param name="meta"></param>
        public EntityMetaEventArgs(EntityMeta meta)
        {
            Meta = meta;
        }
    }
}
