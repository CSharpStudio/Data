using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Mapping
{
    public class TableInfo : ITableInfo
    {
        public TableInfo()
        {
            Columns = new List<ColumnInfo>();
        }

        /// <summary>
        /// 对应的实体类型
        /// </summary>
        public Type Class { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 主键列（每个表肯定有一个主键列）
        /// </summary>
        public ColumnInfo PKColumn { get; set; }

        /// <summary>
        /// 所有的列
        /// </summary>
        public List<ColumnInfo> Columns { get; set; }

        public string ViewSql { get; set; }

        IColumnInfo ITableInfo.PKColumn { get { return PKColumn; } }

        IReadOnlyList<IColumnInfo> ITableInfo.Columns { get { return Columns; } }
    }
}
