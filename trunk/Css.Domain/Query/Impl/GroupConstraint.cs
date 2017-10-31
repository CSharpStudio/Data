using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class GroupConstraint : SqlGroupConstraint, IGroupConstraint
    {
        IConstraint IGroupConstraint.Left
        {
            get
            {
                return base.Left as IConstraint;
            }
            set
            {
                base.Left = value as SqlConstraint;
            }
        }

        GroupOp IGroupConstraint.Opeartor
        {
            get
            {
                return (GroupOp)base.Opeartor;
            }
            set
            {
                base.Opeartor = (SqlGroupOperator)value;
            }
        }

        IConstraint IGroupConstraint.Right
        {
            get
            {
                return base.Right as IConstraint;
            }
            set
            {
                base.Right = value as SqlConstraint;
            }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.GroupConstraint; }
        }
    }
}
