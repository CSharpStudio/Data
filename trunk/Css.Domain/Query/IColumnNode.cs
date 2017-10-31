
using System;

namespace Css.Domain.Query
{
    /// <summary>
    /// 一个列节点
    /// </summary>
    public interface IColumnNode : IQueryNode
    {
        /// <summary>
        /// 本列属于指定的数据源
        /// </summary>
        INamedSource Owner { get; set; }

        /// <summary>
        /// 本属性对应一个实体的托管属性
        /// </summary>
        string PropertyName { get; set; }
        Type PropertyType { get; set; }

        /// <summary>
        /// 本属性在查询结果中使用的别名。
        /// </summary>
        string Alias { get; set; }

        string ColumnName { get; }
    }
}
