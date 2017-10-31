using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 引用的类型。
    /// </summary>
    public enum ReferenceType
    {
        /// <summary>
        /// 一般的外键引用
        /// </summary>
        [Description("一般的引用")]
        Normal,

        /// <summary>
        /// 此引用表示父实体的引用
        /// </summary>
        [Description("父实体的引用")]
        Parent,
    }
}
