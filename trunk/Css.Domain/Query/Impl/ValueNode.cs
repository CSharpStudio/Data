using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Impl
{
    class ValueNode : SqlValue, IValueNode
    {
        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.Value; }
        }
    }
}
