using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Query.Impl
{
    class FunctionNode : SqlFunction, IFunctionNode
    {
        public FunctionNode()
        {
            Args = new ArrayNode();
        }

        IArray IFunctionNode.Args
        {
            get { return base.Args as IArray; }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.Function; }
        }
    }

    class BinaryFunctionNode : SqlBinaryFunction, IFunctionNode
    {
        public BinaryFunctionNode()
        {
            Args = new ArrayNode();
        }

        IArray IFunctionNode.Args
        {
            get { return base.Args as IArray; }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.Function; }
        }
    }
}
