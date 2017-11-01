using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityMetaEventArgs : EventArgs
    {
        public EntityMeta Meta { get; private set; }

        public EntityMetaEventArgs(EntityMeta meta)
        {
            Meta = meta;
        }
    }
}
