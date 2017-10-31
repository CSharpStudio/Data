using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntitySubmittingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySubmittingEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">提交数据的操作类型</param>
        public EntitySubmittingEventArgs(Entity entity, SubmitAction action)
        {
            Entity = entity;
            Action = action;
        }

        /// <summary>
        /// 被操作的实体。
        /// </summary>
        public Entity Entity { get; }

        public SubmitAction Action { get; }
    }
}
