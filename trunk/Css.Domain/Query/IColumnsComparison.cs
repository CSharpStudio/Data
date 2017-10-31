namespace Css.Domain.Query
{
    /// <summary>
    /// 两个列进行对比的约束条件节点
    /// </summary>
    public interface IColumnsComparison : IConstraint
    {
        /// <summary>
        /// 第一个需要对比的列。
        /// </summary>
        IColumnNode LeftColumn { get; set; }

        /// <summary>
        /// 第二个需要对比的列。
        /// </summary>
        IColumnNode RightColumn { get; set; }

        /// <summary>
        /// 对比条件。
        /// </summary>
        BinaryOp Operator { get; set; }
    }
}
