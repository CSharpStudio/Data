using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class Join : SqlJoin, IJoin
    {
        ISource IJoin.Left
        {
            get
            {
                return base.Left as ISource;
            }
            set
            {
                base.Left = value as SqlSource;
            }
        }

        JoinType IJoin.JoinType
        {
            get
            {
                return (JoinType)base.JoinType;
            }
            set
            {
                base.JoinType = (SqlJoinType)value;
            }
        }

        ITableSource IJoin.Right
        {
            get
            {
                return base.Right as ITableSource;
            }
            set
            {
                base.Right = value as SqlTable;
            }
        }

        IConstraint IJoin.Condition
        {
            get
            {
                return base.Condition as IConstraint;
            }
            set
            {
                base.Condition = value as SqlConstraint;
            }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.Join; }
        }

        private TableSourceFinder _finder;

        ITableSource ISource.FindTable(IRepository repo, string alias)
        {
            if (_finder == null) { _finder = new TableSourceFinder(this); }
            return _finder.Find(repo, alias);
        }
    }
}
