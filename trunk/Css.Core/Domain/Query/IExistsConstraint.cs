namespace Css.Domain.Query
{
    /// <summary>
    /// 是否存在查询结果的约束条件节点
    /// </summary>
    public interface IExistsConstraint : IConstraint
    {
        /// <summary>
        /// 要检查的查询。
        /// </summary>
        IQuery Query { get; set; }
        bool IsNot { get; set; }
    }
}