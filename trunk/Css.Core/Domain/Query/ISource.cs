namespace Css.Domain.Query
{
    /// <summary>
    /// 查询的数据源。
    /// </summary>
    public interface ISource : IQueryNode
    {
        /// <summary>
        /// 从当前数据源中查找指定仓库对应的表。
        /// </summary>
        /// <param name="repo">要查找这个仓库对应的表。
        /// 如果这个参数传入 null，则表示查找主表（最左边的表）。</param>
        /// <param name="alias">
        /// 要查找表的别名。
        /// 如果仓库在本数据源中匹配多个表，那么将使用别名来进行精确匹配。
        /// 如果仓库在本数据源中只匹配一个表，那么忽略本参数。
        /// </param>
        /// <returns></returns>
        ITableSource FindTable(IRepository repo = null, string alias = null);
    }
}
