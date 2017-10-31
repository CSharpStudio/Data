using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityAttribute : Attribute
    {
        public EntityAttribute() { }

        public Type RepositoryType { get; set; }

        public Type ListType { get; set; }
    }

    public class RootEntityAttribute : EntityAttribute
    {

    }

    public class ChildEntityAttribute : EntityAttribute
    {

    }
}
