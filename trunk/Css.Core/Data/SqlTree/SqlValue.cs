using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlTree
{
    public class SqlValue : SqlNode, ISqlValue
    {
        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlValue; }
        }

        public object Value { get; set; }
    }
}
