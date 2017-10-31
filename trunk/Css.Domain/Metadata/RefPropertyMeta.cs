using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Metadata
{
    public class RefPropertyMeta : PropertyMeta
    {
        public ReferenceType ReferenceType { get; set; }

        public PropertyInfoMeta RefProperty { get; set; }

        public bool Nullable { get; set; }
    }
}
