using Css.ComponentModel;
using Css.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 实体元数据，映射数据库的信息，继承自<see cref="Extensible"/>
    /// </summary>
    public class EntityMeta : Extensible
    {
        /// <summary>
        /// 实体<see cref="IEntity"/>类型。
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// 表元数据
        /// </summary>
        public TableMeta TableMeta { get; set; }

        /// <summary>
        /// 属性元数据列表
        /// </summary>
        public IList<PropertyMeta> Properties { get; } = new List<PropertyMeta>();

        /// <summary>
        /// 子属性元数据列表
        /// </summary>
        public IList<ChildPropertyMeta> ChildrenProperties { get; } = new List<ChildPropertyMeta>();
    }
}
