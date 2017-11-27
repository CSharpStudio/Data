namespace Css.Data.SqlTree
{
    /// <summary>
    /// 表示一个文本
    /// </summary>
    public class SqlLiteral : SqlConstraint, ISqlLiteral
    {
        public SqlLiteral() { }

        public SqlLiteral(string formattedSql)
        {
            FormattedSql = formattedSql;
        }

        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlLiteral; }
        }

        /// <summary>
        /// Sql 文本。
        /// </summary>
        public string FormattedSql { get; set; }

        /// <summary>
        /// 对应的参数值列表
        /// </summary>
        public object[] Parameters { get; set; }
    }
}
