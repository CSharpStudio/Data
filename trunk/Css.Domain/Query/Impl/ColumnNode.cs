using Css.Data;
using Css.Data.SqlTree;
using System;

namespace Css.Domain.Query.Impl
{
    class ColumnNode : SqlColumn, IColumnNode
    {
        INamedSource IColumnNode.Owner
        {
            get { return base.Table as INamedSource; }
            set { base.Table = value as SqlNamedSource; }
        }

        string IColumnNode.Alias
        {
            get { return base.Alias; }
            set { base.Alias = value; }
        }

        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }

        QueryNodeType IQueryNode.NodeType
        {
            get { return QueryNodeType.Column; }
        }

        internal IColumnInfo DbColumn;
    }
}
