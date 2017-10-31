using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    /// <summary>
    /// 二位运算类型
    /// </summary>
    public enum GroupOp
    {
        /// <summary>
        /// 使用 And 连接。
        /// </summary>
        And = SqlGroupOperator.And,
        /// <summary>
        /// 使用 Or 连接。
        /// </summary>
        Or = SqlGroupOperator.Or
    }
}
