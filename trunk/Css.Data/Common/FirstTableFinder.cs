using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    /// <summary>
    /// 在 Sql 树中找到第一个表。
    /// </summary>
    internal class FirstTableFinder : SqlNodeVisitor
    {
        SqlTable _table;

        public SqlTable Find(ISqlNode node)
        {
            Visit(node);

            return _table;
        }

        protected override ISqlNode Visit(ISqlNode node)
        {
            if (_table != null) { return node; }

            return base.Visit(node);
        }

        protected override SqlTable VisitSqlTable(SqlTable sqlTable)
        {
            if (_table == null)
            {
                _table = sqlTable;
                return sqlTable;
            }

            return base.VisitSqlTable(sqlTable);
        }
    }
}
