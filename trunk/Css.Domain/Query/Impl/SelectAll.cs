using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class SelectAll : SqlSelectAll, ISelectAll
    {
        INamedSource ISelectAll.Source
        {
            get
            {
                return base.Table as INamedSource;
            }
            set
            {
                base.Table = value as SqlNamedSource;
            }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.SelectAll; }
        }
    }
}
