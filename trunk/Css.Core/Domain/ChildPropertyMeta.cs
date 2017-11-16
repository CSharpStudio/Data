using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 子属性元数据，表示聚合或组合子的属性
    /// </summary>
    public class ChildPropertyMeta : PropertyMetadata
    {
        /// <summary>
        /// 一对多属性的类型(聚合、组合)
        /// </summary>
        public HasManyType HasManyType { get; set; }
    }
}
