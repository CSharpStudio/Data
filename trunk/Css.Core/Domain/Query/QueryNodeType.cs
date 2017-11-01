namespace Css.Domain.Query
{
    /// <summary>
    /// 查询树节点的类型。
    /// </summary>
    public enum QueryNodeType
    {
        /// <summary>
        /// 查询结果
        /// </summary>
        Query,
        /// <summary>
        /// 可嵌套的子查询
        /// </summary>
        SubQuery,
        /// <summary>
        /// 节点的数组
        /// </summary>
        Array,
        /// <summary>
        /// 查询的表数据源
        /// </summary>
        TableSource,
        /// <summary>
        /// 表示查询数据源中的所有属性的节点
        /// </summary>
        SelectAll,
        /// <summary>
        /// 数据源与实体数据源连接后的结果节点
        /// </summary>
        Join,
        /// <summary>
        /// 排序节点
        /// </summary>
        OrderBy,
        /// <summary>
        /// 列节点
        /// </summary>
        Column,
        /// <summary>
        /// 二位操作符连接的节点
        /// </summary>
        GroupConstraint,
        /// <summary>
        /// 是否存在查询结果的约束条件节点
        /// </summary>
        ExistsConstraint,
        /// <summary>
        /// 对指定约束条件节点执行取反规则的约束条件节点
        /// </summary>
        NotConstraint,
        /// <summary>
        /// 查询文本
        /// </summary>
        Literal,
        /// <summary>
        /// 分组
        /// </summary>
        GroupBy,
        /// <summary>
        /// 函数
        /// </summary>
        Function,
        /// <summary>
        /// 值
        /// </summary>
        Value,
        /// <summary>
        /// 值对比
        /// </summary>
        BinaryConstraint,
    }
}
