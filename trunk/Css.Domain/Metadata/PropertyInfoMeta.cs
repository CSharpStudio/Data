using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Metadata
{
    public class PropertyInfoMeta
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        public Type DeclareType { get; set; }

        public PropertyCategory Category { get; set; }

        /// <summary>
        /// 是否需要序列化
        /// </summary>
        public bool Serializable { get; set; }

        /// <summary>
        /// 属性所有者的元数据
        /// </summary>
        public EntityMeta Owner { get; set; }
    }
}
