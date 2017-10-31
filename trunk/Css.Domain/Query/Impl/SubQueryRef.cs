using Css.Data.SqlTree;

namespace Css.Domain.Query.Impl
{
    class SubQueryRef : SqlSubSelect, ISubQuery
    {
        IQuery ISubQuery.Query
        {
            get { return base.Select as IQuery; }
            set { base.Select = value as SqlSelect; }
        }

        string ISubQuery.Alias
        {
            get { return base.Alias; }
            set { base.Alias = value; }
        }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.SubQuery; }
        }

        IColumnNode ISubQuery.Column(IColumnNode rawProperty)
        {
            var raw = rawProperty as ColumnNode;

            var property = new ColumnNode
            {
                ColumnName = raw.ColumnName,
                PropertyName = raw.PropertyName,
                PropertyType = raw.PropertyType,
                Table = this,
            };

            return property;
        }

        IColumnNode ISubQuery.Column(IColumnNode rawProperty, string alias)
        {
            var raw = rawProperty as ColumnNode;

            var property = new ColumnNode
            {
                ColumnName = raw.ColumnName,
                PropertyName = raw.PropertyName,
                PropertyType = raw.PropertyType,
                Table = this,
                Alias = alias
            };

            return property;
        }

        string INamedSource.GetName()
        {
            return base.GetName();
        }

        TableSourceFinder _finder;

        ITableSource ISource.FindTable(IRepository repo, string alias)
        {
            if (_finder == null) { _finder = new TableSourceFinder(this); }
            return _finder.Find(repo, alias);
        }
    }
}
