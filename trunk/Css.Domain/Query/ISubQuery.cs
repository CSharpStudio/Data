namespace Css.Domain.Query
{
    /// <summary>
    /// 子查询。
    /// 对一个子查询分配别名后，可以作为一个新的源。
    /// </summary>
    public interface ISubQuery : INamedSource
    {
        /// <summary>
        /// 内部的查询对象。
        /// </summary>
        IQuery Query { get; set; }

        /// <summary>
        /// 必须对这个子查询指定别名。
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// 为这个子查询结果中的某个列来生成一个属于这个 ISubQueryRef 对象的结果列。
        /// </summary>
        /// <param name="rawColumn">子查询结果中的某个列。</param>
        /// <returns></returns>
        IColumnNode Column(IColumnNode rawColumn);

        /// <summary>
        /// 为这个子查询结果中的某个列来生成一个属于这个 ISubQueryRef 对象的结果列。
        /// 同时，设置它的查询的别名。
        /// </summary>
        /// <param name="rawColumn">子查询结果中的某个列。</param>
        /// <param name="alias">别名。</param>
        /// <returns></returns>
        IColumnNode Column(IColumnNode rawColumn, string alias);
    }
}