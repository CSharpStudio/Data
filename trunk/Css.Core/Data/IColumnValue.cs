using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data
{
    public interface IColumnValue
    {
        /// <summary>
        /// 列
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        object Value { get; set; }
    }

    public class ColumnValue : IColumnValue
    {
        public string PropertyName { get; set; }

        public object Value { get; set; }
    }
}
