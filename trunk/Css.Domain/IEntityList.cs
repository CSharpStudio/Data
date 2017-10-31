using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public interface IEntityList : IList, IDomain
    {
        Type EntityType { get; }

        void SyncParentEntityId(Entity id);

        IList Deleted { get; }

        int Total { get; set; }
    }
}
