using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 提交数据的操作类型
    /// </summary>
    public enum SubmitAction
    {
        /// <summary>
        /// 更新实体
        /// 将会执行 Update、SubmitChildren、SubmitTreeChildren。
        /// </summary>
        Update,
        /// <summary>
        /// 插入实体
        /// 将会执行 Insert、SubmitChildren、SubmitTreeChildren。
        /// </summary>
        Insert,
        /// <summary>
        /// 删除实体
        /// 将会执行 DeleteChildren、DeleteTreeChildren、Delete
        /// </summary>
        Delete,
        /// <summary>
        /// 当前对象未变更，只提交其中的子对象。
        /// 将会执行 UpdateChildren、UpdateTreeChildren。
        /// </summary>
        ChildrenOnly
    }
}
