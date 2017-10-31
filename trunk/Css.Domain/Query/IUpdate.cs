using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    /// <summary>
    /// SQL执行接口
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// 获取这个查询中的主实体数据源。
        /// </summary>
        ITableSource MainTable { get; }

        /// <summary>
        /// 查询的过滤条件。
        /// </summary>
        IConstraint Where { get; set; }
    }
    /// <summary>
    /// SQL更新接口
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// 更新字段
        /// </summary>
        IList<IColumnValue> Columns { get; set; }
    }
    /// <summary>
    /// SQL删除接口
    /// </summary>
    public interface IDelete
    {
    }

    internal class Update : IUpdate
    {
        public IList<IColumnValue> Columns { get; set; }

        public ITableSource MainTable { get; set; }

        public IConstraint Where { get; set; }
    }

    internal class Delete : IDelete
    {
        public ITableSource MainTable { get; set; }

        public IConstraint Where { get; set; }
    }
}
