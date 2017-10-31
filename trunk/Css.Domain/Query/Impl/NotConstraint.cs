using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class NotConstraint : SqlNotConstraint, INotConstraint
    {
        IConstraint INotConstraint.Constraint
        {
            get
            {
                return base.Constraint as IConstraint;
            }
            set
            {
                base.Constraint = value as SqlConstraint;
            }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.NotConstraint; }
        }
    }
}
