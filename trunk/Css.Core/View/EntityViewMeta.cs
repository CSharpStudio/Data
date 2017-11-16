using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.View
{
    public class EntityViewMeta : Extensible
    {

        /// <summary>
        /// 当前模型是对应这个类型的。
        /// </summary>
        public Type EntityType { get; set; }

        public IList<PropertyViewMeta> Properties { get; } = new List<PropertyViewMeta>();

        public IList<ChildPropertyViewMeta> ChildrenProperties { get; } = new List<ChildPropertyViewMeta>();
    }
}
