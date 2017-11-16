using Css.Data;

namespace Css.Domain.Query
{
    public interface INodeConstraint : IConstraint
    {
        /// <summary>
        /// 要对比的列。
        /// </summary>
        IQueryNode Node { get; set; }

        /// <summary>
        /// 对比操作符
        /// </summary>
        BinaryOp GetOperator();

        /// <summary>
        /// 对比操作符
        /// </summary>
        void SetOperator(BinaryOp value);

        /// <summary>
        /// 要对比的值。
        /// </summary>
        object Value { get; set; }
    }
}
