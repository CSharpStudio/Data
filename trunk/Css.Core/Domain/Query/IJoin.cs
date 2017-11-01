namespace Css.Domain.Query
{
    /// <summary>
    /// 数据源与实体数据源连接后的结果节点
    /// </summary>
    public interface IJoin : ISource
    {
        /// <summary>
        /// 左边需要连接的数据源。
        /// </summary>
        ISource Left { get; set; }

        /// <summary>
        /// 连接方式
        /// </summary>
        JoinType JoinType { get; set; }

        /// <summary>
        /// 右边需要连接的数据源。
        /// </summary>
        ITableSource Right { get; set; }

        /// <summary>
        /// 连接所使用的约束条件。
        /// </summary>
        IConstraint Condition { get; set; }
    }

    /// <summary>
    /// 支持的连接方式。
    /// </summary>
    public enum JoinType
    {
        /// <summary>
        /// 内连接
        /// </summary>
        Inner,
        /// <summary>
        /// 左外连接
        /// </summary>
        LeftOuter,
        /// <summary>
        /// 右外连接
        /// </summary>
        RightOuter
    }
}