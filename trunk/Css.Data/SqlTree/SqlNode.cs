using Css.Data.SqlClient;
using System.Diagnostics;

namespace Css.Data.SqlTree
{
    /// <summary>
    /// 表示 Sql 语法树中的一个节点。
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract class SqlNode : ISqlNode
    {
        /// <summary>
        /// 返回当前树节点的类型。
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        public abstract SqlNodeType NodeType { get; }

        string DebuggerDisplay
        {
            get
            {
                var generator = new SqlServerSqlGenerator();
                generator.Generate(this);
                return string.Format(generator.Sql, generator.Sql.Parameters);
            }
        }
    }

    /// <summary>
    /// 语法树节点类型。
    /// </summary>
    public enum SqlNodeType
    {
        SqlNodeList,
        SqlLiteral,
        SqlArray,
        SqlSelect,
        SqlTable,
        SqlColumn,
        SqlJoin,
        SqlOrderBy,
        SqlOrderByList,
        SqlSelectAll,
        SqlSubSelect,
        SqlGroupConstraint,
        SqlExistsConstraint,
        SqlNotConstraint,
        SqlGroupBy,
        SqlGroupByList,
        SqlFunction,
        SqlValue,
        SqlBinaryConstraint,
        SqlBinaryFunctioin
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
        //XXXXXXXXXXXXXXXXXXX,
    }
}