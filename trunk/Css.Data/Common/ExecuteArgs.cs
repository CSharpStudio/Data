using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    /// <summary>
    /// 执行参数
    /// </summary>
    public class ExecuteArgs
    {
        public ExecuteArgs(ExecuteType type, SqlTable mainTable, ISqlConstraint where, IList<IColumnValue> columns = null)
        {
            Type = type;
            MainTable = mainTable;
            Where = where;
            if (type == ExecuteType.Update)
                Columns.AddRange(columns);
        }
        public ExecuteType Type { get; set; }
        /// <summary>
        /// 获取这个查询中的主实体数据源。
        /// </summary>
        public SqlTable MainTable { get; }

        /// <summary>
        /// 查询的过滤条件。
        /// </summary>
        public ISqlConstraint Where { get; set; }

        public List<IColumnValue> Columns { get; } = new List<IColumnValue>();
    }
}
