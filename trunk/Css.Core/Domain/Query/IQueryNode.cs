using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    /// <summary>
    /// 表示实体查询语法树中的一个节点。
    /// </summary>
    public interface IQueryNode
    {
        /// <summary>
        /// 节点的类型。
        /// </summary>
        QueryNodeType NodeType { get; }
    }
}
