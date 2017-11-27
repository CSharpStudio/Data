namespace Css.Data.SqlTree
{
    /// <summary>
    /// 排序结点。
    /// </summary>
    public class SqlGroupBy : SqlNode, IGroupBy
    {
        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlGroupBy; }
        }

        /// <summary>
        /// 使用这个列进行排序。
        /// </summary>
        public SqlColumn Column { get; set; }
    }
}
