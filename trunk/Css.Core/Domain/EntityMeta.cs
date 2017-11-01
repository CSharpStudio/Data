using Css.ComponentModel;
using Css.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityMeta
    {
        /// <summary>
        /// 当前模型是对应这个类型的。
        /// </summary>
        public Type EntityType { get; set; }

        public TableMeta TableMeta { get; set; }

        public IList<PropertyMeta> Properties { get; } = new List<PropertyMeta>();

        public IList<ChildPropertyMeta> ChildrenProperties { get; } = new List<ChildPropertyMeta>();

        public RefPropertyMeta FindParentProperty()
        {
            return Properties.OfType<RefPropertyMeta>().FirstOrDefault(p => p.ReferenceType == ReferenceType.Parent);
        }
    }
}
