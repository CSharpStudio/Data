using Css.Data;
using Css.Data.SqlTree;
using System.ComponentModel;

namespace Css.Domain.Query.Impl
{
    class OrderBy : SqlOrderBy, IOrderBy
    {
        IColumnNode IOrderBy.Column
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

        ListSortDirection IOrderBy.Direction
        {
            get
            {
                return base.Direction;
            }
            set
            {
                base.Direction = value;
            }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.OrderBy; }
        }
    }
}
