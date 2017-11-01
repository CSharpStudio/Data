using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 一对多子属性的类型
    /// </summary>
    public enum HasManyType
    {
        /// <summary>
        /// 组合（子对象）
        /// </summary>
        [Description("组合")]
        Composition,

        /// <summary>
        /// 聚合（简单引用）
        /// </summary>
        [Description("聚合")]
        Aggregation
    }
}
