namespace Css.Domain.Query
{
    /// <summary>
    /// 二位操作符连接的节点
    /// </summary>
    public interface IGroupConstraint : IConstraint
    {
        /// <summary>
        /// 二位运算的左操作结点。
        /// </summary>
        IConstraint Left { get; set; }

        /// <summary>
        /// 二位运算类型。
        /// </summary>
        GroupOp Opeartor { get; set; }

        /// <summary>
        /// 二位运算的右操作节点。
        /// </summary>
        IConstraint Right { get; set; }
    }
}