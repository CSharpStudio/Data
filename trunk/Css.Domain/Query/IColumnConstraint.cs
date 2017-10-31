namespace Css.Domain.Query
{
    /// <summary>
    /// 列的约束条件节点
    /// </summary>
    public interface IColumnConstraint : IConstraint
    {
        /// <summary>
        /// 要对比的列。
        /// </summary>
        IColumnNode Column { get; set; }

        /// <summary>
        /// 对比操作符
        /// </summary>
        BinaryOp Operator { get; set; }

        /// <summary>
        /// 要对比的值。
        /// </summary>
        object Value { get; set; }
    }
}
