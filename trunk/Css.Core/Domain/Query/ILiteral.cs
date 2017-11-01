namespace Css.Domain.Query
{
    /// <summary>
    /// 查询文本。
    /// 查询文本可以表示一个条件。
    /// </summary>
    public interface ILiteral : IQueryNode, IConstraint
    {
        /// <summary>
        /// 查询文本。
        /// </summary>
        string FormattedSql { get; set; }

        /// <summary>
        /// 对应的参数值列表
        /// </summary>
        object[] Parameters { get; set; }
    }
}