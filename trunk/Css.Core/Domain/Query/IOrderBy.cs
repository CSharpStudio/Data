using Css.Data;
using System.ComponentModel;

namespace Css.Domain.Query
{
    /// <summary>
    /// 排序节点
    /// </summary>
    public interface IOrderBy : IQueryNode
    {
        /// <summary>
        /// 使用这个属性进行排序。
        /// </summary>
        IColumnNode Column { get; set; }

        /// <summary>
        /// 使用这个方向进行排序。
        /// </summary>
        ListSortDirection Direction { get; set; }
    }
}
