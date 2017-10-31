using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Resources
{
    public interface IResourceObject
    {
        string CultureCode { get; set; }
        string Name { get; set; }
        object Value { get; set; }
    }

    public class ResourceObject : IResourceObject
    {
        public string CultureCode { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }
    }
}
