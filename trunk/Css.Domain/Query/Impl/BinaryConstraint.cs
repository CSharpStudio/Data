using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Impl
{
    class BinaryConstraint : SqlBinaryConstraint, IBinaryConstraint
    {
        IQueryNode IBinaryConstraint.Left
        {
            get { return base.Left as IQueryNode; }
            set { base.Left = value as SqlNode; }
        }

        IQueryNode IBinaryConstraint.Right
        {
            get { return base.Right as IQueryNode; }
            set { base.Right = value as SqlNode; }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.BinaryConstraint; }
        }
    }
}
