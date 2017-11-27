namespace Css.Data.SqlTree
{
    /// <summary>
    /// 表示对指定的查询进行是否存在查询行的逻辑的判断。
    /// </summary>
    public class SqlExistsConstraint : SqlConstraint
    {
        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlExistsConstraint; }
        }

        /// <summary>
        /// 要检查的查询。
        /// </summary>
        public SqlSelect Select { get; set; }

        public bool IsNot { get; set; }
    }
}
