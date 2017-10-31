using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class ExistsConstraint : SqlExistsConstraint, IExistsConstraint
    {
        IQuery IExistsConstraint.Query
        {
            get
            {
                return base.Select as IQuery;
            }
            set
            {
                base.Select = value as SqlSelect;
            }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.ExistsConstraint; }
        }
    }
}
