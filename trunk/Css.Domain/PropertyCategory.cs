using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// SIE 中可用的属性类型
    /// </summary>
    public enum PropertyCategory
    {
        /// <summary>
        /// 一般属性
        /// </summary>
        [Description("一般属性")]
        Normal,
        /// <summary>
        /// 引用属性
        /// </summary>
        [Description("引用属性的ID")]
        ReferenceId,
        /// <summary>
        /// 引用属性
        /// </summary>
        [Description("引用属性")]
        ReferenceEntity,
        /// <summary>
        /// 列表属性
        /// </summary>
        [Description("列表属性")]
        List,
        /// <summary>
        /// 只读属性
        /// </summary>
        [Description("只读属性")]
        Readonly,
        /// <summary>
        /// 冗余属性
        /// </summary>
        [Description("冗余属性")]
        Redundancy,
    }
}
