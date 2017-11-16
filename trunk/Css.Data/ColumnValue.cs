using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Data
{
    /// <summary>
    /// 列值
    /// </summary>
    public class ColumnValue : IColumnValue
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public object Value { get; set; }
    }
}
