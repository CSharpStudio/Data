using System.Collections.Generic;

namespace Css.Domain.Query
{
    /// <summary>
    /// 节点的数组
    /// </summary>
    public interface IArray : IQueryNode
    {
        /// <summary>
        /// 集合中的所有项。
        /// </summary>
        IList<IQueryNode> Items { get; set; }
    }
}
