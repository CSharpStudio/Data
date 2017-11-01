using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        BinaryOp Operator { get; set; }

        /// <summary>
        /// 要对比的值。
        /// </summary>
        object Value { get; set; }
    }
}
