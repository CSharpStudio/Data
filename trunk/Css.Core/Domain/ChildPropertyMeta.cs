using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class ChildPropertyMeta : PropertyInfoMeta
    {
        /// <summary>
        /// 一对多属性的类型(聚合、组合)
        /// </summary>
        public HasManyType HasManyType { get; set; }
    }
}
