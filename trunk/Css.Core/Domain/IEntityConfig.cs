using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public interface IEntityConfig
    {
        EntityMeta Meta { get; set; }
        void ConfigMeta();
        Type EntityType { get; }
    }
}
