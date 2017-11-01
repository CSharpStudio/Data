using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query
{
    public interface IValueNode : IQueryNode
    {
        object Value { get; set; }
    }
}
