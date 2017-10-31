using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityConvention
    {
        /// <summary>
        /// 需要手动影射栏位的属性，在MapAllProperties()和MapAllPropertiesExcept()中不会匹配这些属性
        /// </summary>
        public static IList<IProperty> ExceptMapColumnProperties { get; } = new List<IProperty>();
    }
}
