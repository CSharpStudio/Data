using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class Literal : SqlLiteral, ILiteral
    {
        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.Literal; }
        }
    }
}
