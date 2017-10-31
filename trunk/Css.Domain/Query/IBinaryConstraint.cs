using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    public interface IBinaryConstraint : IConstraint
    {
        /// <summary>
        /// 第一个需要对比的列。
        /// </summary>
        IQueryNode Left { get; set; }

        /// <summary>
        /// 第二个需要对比的列。
        /// </summary>
        IQueryNode Right { get; set; }

        /// <summary>
        /// 对比条件。
        /// </summary>
        BinaryOp Operator { get; set; }
    }
}