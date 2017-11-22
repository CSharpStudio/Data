using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.View
{
    /// <summary>
    /// 实体视图元数据
    /// </summary>
    public class EntityViewMeta : Extensible
    {
        /// <summary>
        /// 当前模型是对应这个类型的。
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// 属性视图元数据集合
        /// </summary>
        public IList<PropertyViewMeta> Properties { get; } = new List<PropertyViewMeta>();

        /// <summary>
        /// 子属性视图元数据集合
        /// </summary>
        public IList<ChildPropertyViewMeta> ChildrenProperties { get; } = new List<ChildPropertyViewMeta>();
    }
}
