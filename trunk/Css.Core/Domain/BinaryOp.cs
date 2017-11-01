namespace Css.Domain
{
    /// <summary>
    /// 属性的对比操作符
    /// </summary>
    public enum BinaryOp
    {
        Equal,
        NotEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,

        Like,
        NotLike,
        Contains,
        NotContains,
        StartsWith,
        NotStartsWith,
        EndsWith,
        NotEndsWith,

        In,
        NotIn,
    }
}
