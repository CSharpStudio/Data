namespace Css.Domain.Query
{
    /// <summary>
    /// 对指定约束条件节点执行取反规则的约束条件节点
    /// </summary>
    public interface INotConstraint : IConstraint
    {
        /// <summary>
        /// 需要被取反的条件。
        /// </summary>
        IConstraint Constraint { get; set; }
    }
}
