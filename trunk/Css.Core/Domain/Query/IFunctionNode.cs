using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    /// <summary>
    /// 函数
    /// </summary>
    public interface IFunctionNode : IQueryNode
    {
        /// <summary>
        /// 参数
        /// </summary>
        IArray Args { get; }

        /// <summary>
        /// 本属性在查询结果中使用的别名。
        /// </summary>
        string Alias { get; set; }

        string Function { get; set; }
    }
}
