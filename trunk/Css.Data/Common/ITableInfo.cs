using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    public interface ITableInfo
    {
        /// <summary>
        /// 对应的实体类型
        /// </summary>
        Type Class { get; }

        /// <summary>
        /// 表名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 视图查询语句
        /// </summary>
        string ViewSql { get; }

        /// <summary>
        /// 主键列（每个表肯定有一个主键列）
        /// </summary>
        IColumnInfo PKColumn { get; }

        /// <summary>
        /// 所有的列
        /// </summary>
        IReadOnlyList<IColumnInfo> Columns { get; }
    }
}
