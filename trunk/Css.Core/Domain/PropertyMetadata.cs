using Css.ComponentModel;
using System;

namespace Css.Domain
{
    /// <summary>
    /// 属性信息元数据，继承自<see cref="Extensible"/>
    /// </summary>
    public class PropertyMetadata : Extensible
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性类型
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// 属性声明类型
        /// </summary>
        public Type DeclareType { get; set; }

        /// <summary>
        /// 是否需要序列化
        /// </summary>
        public bool Serializable { get; set; }

        /// <summary>
        /// 属性所有者<see cref="IEntity"/>的元数据
        /// </summary>
        public EntityMeta Owner { get; set; }
    }
}
