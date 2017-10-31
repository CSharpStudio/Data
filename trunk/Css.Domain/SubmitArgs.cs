using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 提交参数。
    /// </summary>
    public struct SubmitArgs
    {
        /// <summary>
        /// 被保存的领域实体
        /// </summary>
        public Entity Entity { get; internal set; }

        /// <summary>
        /// 保存的操作
        /// </summary>
        public SubmitAction Action { get; internal set; }

        /// <summary>
        /// 标记本次操作，不但要保存所有的子实体，也要保存当前对象。
        /// 场景：
        /// 子类重写 Submit 方法后，在当前实体数据不脏、只更新组合子实体（ChildrenOnly）的模式下，
        /// 如果修改了当前实体的状态，则需要使用这个方法把提交操作提升为保存整个组合对象（Update），这样当前实体才会被保存。
        /// 
        /// 注意，由于 SubmitArgs 是一个结构体，所以调用此方法只会更改当前对象的值。需要把这个改了值的对象传入基类的方法，才能真正地更新当前的实体对象。
        /// </summary>
        /// <exception cref="System.InvalidOperationException">只有在 ChildrenOnly 模式下，才可以调用此方法。</exception>
        public void UpdateCurrent()
        {
            if (Action != SubmitAction.ChildrenOnly)
            {
                throw new InvalidOperationException("只有在 ChildrenOnly 模式下，才可以调用此方法。");
            }

            Action = SubmitAction.Update;
        }
    }
}
