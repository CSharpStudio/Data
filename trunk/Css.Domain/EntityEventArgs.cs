using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="EntityEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityEventArgs(Entity entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// 被操作的实体。
        /// </summary>
        public Entity Entity { get; private set; }
    }
}
