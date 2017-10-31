using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class GroupBy : SqlGroupBy, IGroupBy
    {
        IColumnNode IGroupBy.Column
        {
            get
            {
                return base.Column as IColumnNode;
            }
            set
            {
                base.Column = value as SqlColumn;
            }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.GroupBy; }
        }
    }
}
