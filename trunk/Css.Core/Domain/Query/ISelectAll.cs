namespace Css.Domain.Query
{
    /// <summary>
    /// 表示查询数据源中的所有属性的节点、也可以表示查询某个指定数据源的所有属性。
    /// </summary>
    public interface ISelectAll : IQueryNode
    {
        /// <summary>
        /// 如果本属性为空，表示选择所有数据源的所有属性；否则表示选择指定数据源的所有属性。
        /// </summary>
        INamedSource Source { get; set; }
    }
}
