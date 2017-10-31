using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Metadata
{
    public class PropertyMeta : PropertyInfoMeta
    {
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadonly { get; set; }

        /// <summary>
        /// 数据列元数据
        /// </summary>
        public ColumnMeta ColumnMeta { get; set; }
    }
}
