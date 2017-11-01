using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Domain
{
    public interface IEntityMetaCreator
    {
        EntityMeta Create(Type type);
    }
}
