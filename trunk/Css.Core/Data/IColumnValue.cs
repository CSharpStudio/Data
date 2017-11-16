using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data
{
    /// <summary>
    /// 列值
    /// </summary>
    public interface IColumnValue
    {
        /// <summary>
        /// 属性名
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        object Value { get; set; }
    }
}
