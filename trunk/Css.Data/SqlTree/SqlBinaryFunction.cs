using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlTree
{
    public class SqlBinaryFunction : SqlFunction
    {
        public override SqlNodeType NodeType
        {
            get { return SqlNodeType.SqlBinaryFunctioin; }
        }

        public SqlNode Left { get { return Args.Items?.OfType<SqlNode>().FirstOrDefault(); } }

        public SqlNode Right { get { return Args.Items?.OfType<SqlNode>().LastOrDefault(); } }
    }
}
