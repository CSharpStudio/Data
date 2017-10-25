using Css.Data.SqlTree;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    /// <summary>
    /// 为 SqlNode 语法树生成相应 Sql 的生成器。
    /// </summary>
    public abstract class SqlGenerator : SqlNodeVisitor
    {
        internal const string WILDCARD_ALL = "%";
        internal const string WILDCARD_SINGLE = "_";
        internal const string ESCAPE_CHAR = "\\";
        internal static readonly string WILDCARD_ALL_ESCAPED = ESCAPE_CHAR + WILDCARD_ALL;
        internal static readonly string WILDCARD_SINGLE_ESCAPED = ESCAPE_CHAR + WILDCARD_SINGLE;

        FormattedSql _sql;

        public SqlGenerator()
        {
            _sql = new FormattedSql();
            _sql.InnerWriter = new IndentedTextWriter(_sql.InnerWriter);
            AutoQuota = true;
            MaxItemsInInClause = int.MaxValue;
        }

        /// <summary>
        /// 当前需要的缩进量。
        /// </summary>
        protected int Indent
        {
            get { return (_sql.InnerWriter as IndentedTextWriter).Indent; }
            set { (_sql.InnerWriter as IndentedTextWriter).Indent = value; }
        }

        /// <summary>
        /// 是否自动添加标识符的括号
        /// </summary>
        public bool AutoQuota { get; set; }

        bool _columnAlias = true;

        bool _useParameter = true;

        /// <summary>
        /// 生成完毕后的 Sql 语句及参数。
        /// </summary>
        public FormattedSql Sql
        {
            get { return _sql; }
        }

        /// <summary>
        /// In 语句中可以承受的最大的个数。
        /// 如果超出这个个数，则会抛出
        /// </summary>
        internal int MaxItemsInInClause;

        #region 分页支持

        /// <summary>
        /// 为指定的原始查询生成指定分页效果的新查询。
        /// </summary>
        /// <param name="raw">原始查询</param>
        /// <param name="startRow">开始行号</param>
        /// <param name="endRow">结束行号</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">pagingInfo</exception>
        /// <exception cref="System.InvalidProgramException">必须排序后才能使用分页功能。</exception>
        protected virtual ISqlSelect ModifyToPagingTree(SqlSelect raw, int startRow, int endRow)
        {
            if (!raw.HasOrdered()) { throw new InvalidProgramException("必须排序后才能使用分页功能。"); }

            /*********************** 代码块解释 *********************************
             * 
             * 使用 ROW_NUMBER() 函数，此函数 SqlServer、Oracle 都可使用。
             * 注意，这个方法只支持不太复杂 SQL 的转换。
             *
             * 源格式：
             * select ...... from ...... order by xxxx asc, yyyy desc
             * 不限于以上格式，只要满足没有复杂的嵌套查询，最外层是一个 Select 和 From 语句即可。
             * 
             * 目标格式：
             * select * from (select ......, row_number() over(order by xxxx asc, yyyy desc) _rowNumber from ......) x where x._rowNumber<10 and x._rowNumber>5;
            **********************************************************************/

            var innerSelect = new SqlSelect();
            var selection = new SqlArray();
            if (raw.Selection != null)
            {
                selection.Items.Add(raw.Selection);
            }
            selection.Items.Add(new SqlNodeList
            {
                new SqlLiteral { FormattedSql = "row_number() over (" },
                raw.OrderBy,
                new SqlLiteral { FormattedSql = ") _rowNumber" }
            });
            innerSelect.Selection = selection;

            var subSelect = new SqlSubSelect
            {
                Select = innerSelect,
                Alias = "x"
            };
            var rowNumberColumn = new SqlColumn
            {
                Table = subSelect,
                ColumnName = "_rowNumber"
            };
            var pagingSelect = new SqlSelect();
            pagingSelect.From = subSelect;
            pagingSelect.Where = new SqlGroupConstraint
            {
                Left = new SqlBinaryConstraint
                {
                    Left = rowNumberColumn,
                    Operator = SqlBinaryOperator.GreaterEqual,
                    Right = new SqlValue { Value = startRow }
                },
                Opeartor = SqlGroupOperator.And,
                Right = new SqlBinaryConstraint
                {
                    Left = rowNumberColumn,
                    Operator = SqlBinaryOperator.LessEqual,
                    Right = new SqlValue { Value = endRow }
                }
            };

            return pagingSelect;
        }

        #endregion

        /// <summary>
        /// 访问 sql 语法树中的每一个结点，并生成相应的 Sql 语句。
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="start">The paging information.</param>
        /// <param name="end">The paging information.</param>
        public void Generate(SqlSelect tree, int start = 0, int end = 0)
        {
            ISqlSelect res = tree;
            if (start > 0 || end > 0)
            {
                res = ModifyToPagingTree(tree, start, end);
            }

            base.Visit(res);
        }

        /// <summary>
        /// 访问 sql 语法树中的每一个结点，并生成相应的 Sql 语句。
        /// </summary>
        /// <param name="tree">The tree.</param>
        public void Generate(SqlNode tree)
        {
            base.Visit(tree);
        }

        protected override SqlLiteral VisitSqlLiteral(SqlLiteral sqlLiteral)
        {
            if (sqlLiteral.Parameters != null && sqlLiteral.Parameters.Length > 0)
            {
                sqlLiteral.FormattedSql = Regex.Replace(sqlLiteral.FormattedSql, @"\{(?<index>\d+)\}", m =>
                {
                    var index = Convert.ToInt32(m.Groups["index"].Value);
                    var value = sqlLiteral.Parameters[index];
                    index = _sql.Parameters.Add(value);
                    return "{" + index + "}";
                });
            }

            _sql.Append(sqlLiteral.FormattedSql);
            return sqlLiteral;
        }

        protected override SqlGroupOperator VisitGroupOperator(SqlGroupOperator op)
        {
            switch (op)
            {
                case SqlGroupOperator.And:
                    _sql.AppendAnd();
                    break;
                case SqlGroupOperator.Or:
                    _sql.AppendOr();
                    break;
                default:
                    throw new NotSupportedException();
            }
            return op;
        }

        protected override SqlGroupConstraint VisitSqlGroupConstraint(SqlGroupConstraint node)
        {
            if (node.Opeartor == SqlGroupOperator.And)
            {
                var leftBinary = node.Left as SqlGroupConstraint;
                var rightBinary = node.Right as SqlGroupConstraint;
                var isLeftOr = leftBinary != null && leftBinary.Opeartor == SqlGroupOperator.Or;
                var isRightOr = rightBinary != null && rightBinary.Opeartor == SqlGroupOperator.Or;

                if (isLeftOr) _sql.Append("(");
                Visit(node.Left);
                if (isLeftOr) _sql.Append(")");
                VisitGroupOperator(node.Opeartor);
                if (isRightOr) _sql.Append("(");
                Visit(node.Right);
                if (isRightOr) _sql.Append(")");
            }
            else base.VisitSqlGroupConstraint(node);
            return node;
        }

        protected override SqlSelect VisitSqlSelect(SqlSelect sqlSelect)
        {
            _sql.Append("SELECT ");

            //SELECT
            GenerateSelection(sqlSelect);

            //FROM
            _sql.AppendLine();
            _sql.Append("FROM ");
            Visit(sqlSelect.From);

            //WHERE
            if (sqlSelect.Where != null)
            {
                _sql.AppendLine();
                _sql.Append("WHERE ");
                Visit(sqlSelect.Where);
            }

            if (sqlSelect.HasGroup())
            {
                _sql.AppendLine();
                Visit(sqlSelect.GroupBy);
            }


            //WHERE
            if (sqlSelect.Having != null)
            {
                _sql.AppendLine();
                _sql.Append("HAVING ");
                Visit(sqlSelect.Having);
            }

            //ORDER BY
            if (!sqlSelect.IsCounting && sqlSelect.OrderBy != null && sqlSelect.OrderBy.Count > 0)
            {
                _sql.AppendLine();
                Visit(sqlSelect.OrderBy);
            }

            return sqlSelect;
        }

        /// <summary>
        /// 生成 Selection 中的语句
        /// </summary>
        /// <param name="sqlSelect"></param>
        protected virtual void GenerateSelection(SqlSelect sqlSelect)
        {
            if (sqlSelect.IsCounting)
            {
                _sql.Append("COUNT(0)");
            }
            else
            {
                if (sqlSelect.IsDistinct)
                {
                    _sql.Append("DISTINCT ");
                }

                if (sqlSelect.Selection == null)
                {
                    //默认约定第一张表，就是
                    var table = new FirstTableFinder().Find(sqlSelect.From);
                    var selection = new SqlSelectAll
                    {
                        Table = table
                    };
                    Visit(selection);
                    //_sql.Append("*");
                }
                else
                {
                    Visit(sqlSelect.Selection);
                }
            }
        }

        protected override SqlColumn VisitSqlColumn(SqlColumn sqlColumn)
        {
            AppendColumnUsage(sqlColumn);

            if (_columnAlias && !string.IsNullOrEmpty(sqlColumn.Alias))
            {
                AppendNameCast();
                QuoteAppend(sqlColumn.Alias);
            }
            return sqlColumn;
        }

        protected override SqlTable VisitSqlTable(SqlTable sqlTable)
        {
            QuoteAppend(sqlTable.TableName);
            if (!string.IsNullOrEmpty(sqlTable.Alias))
            {
                AppendNameCast();
                QuoteAppend(sqlTable.Alias);
            }

            return sqlTable;
        }

        protected override SqlJoin VisitSqlJoin(SqlJoin sqlJoin)
        {
            Visit(sqlJoin.Left);

            switch (sqlJoin.JoinType)
            {
                //case SqlJoinType.Cross:
                //    _sql.Append(", ");
                //    break;
                case SqlJoinType.Inner:
                    _sql.AppendLine();
                    Indent++;
                    _sql.Append("INNER JOIN ");
                    Indent--;
                    break;
                case SqlJoinType.LeftOuter:
                    _sql.AppendLine();
                    Indent++;
                    _sql.Append("LEFT OUTER JOIN ");
                    Indent--;
                    break;
                case SqlJoinType.RightOuter:
                    _sql.AppendLine();
                    Indent++;
                    _sql.Append("RIGHT OUTER JOIN ");
                    Indent--;
                    break;
                default:
                    throw new NotSupportedException();
            }

            Visit(sqlJoin.Right);

            _sql.Append(" ON ");

            Visit(sqlJoin.Condition);

            return sqlJoin;
        }

        protected override SqlArray VisitSqlArray(SqlArray sqlArray)
        {
            for (int i = 0, c = sqlArray.Items.Count; i < c; i++)
            {
                var item = sqlArray.Items[i] as SqlNode;
                if (i > 0)
                {
                    _sql.Append(", ");
                }
                Visit(item);
            }

            return sqlArray;
        }

        protected override SqlBinaryOperator VisitBinaryOperator(SqlBinaryOperator op)
        {
            switch (op)
            {
                case SqlBinaryOperator.Equal: _sql.Append(" = "); break;
                case SqlBinaryOperator.NotEqual: _sql.Append(" != "); break;
                case SqlBinaryOperator.Greater: _sql.Append(" > "); break;
                case SqlBinaryOperator.GreaterEqual: _sql.Append(" >= "); break;
                case SqlBinaryOperator.Less: _sql.Append(" < "); break;
                case SqlBinaryOperator.LessEqual: _sql.Append(" <= "); break;
                case SqlBinaryOperator.Like:
                case SqlBinaryOperator.Contains:
                case SqlBinaryOperator.StartsWith:
                case SqlBinaryOperator.EndsWith: _sql.Append(" LIKE "); break;
                case SqlBinaryOperator.NotLike:
                case SqlBinaryOperator.NotContains:
                case SqlBinaryOperator.NotStartsWith:
                case SqlBinaryOperator.NotEndsWith: _sql.Append(" NOT LIKE "); break;
                case SqlBinaryOperator.In: _sql.Append(" IN "); break;
                case SqlBinaryOperator.NotIn: _sql.Append(" NOT IN "); break;
            }
            return op;
        }

        bool VisitSpecialBinary(SqlBinaryConstraint node)
        {
            var op = node.Operator;
            var isNullValue = IsNullValue(node.Right);
            switch (op)
            {
                case SqlBinaryOperator.Contains:
                case SqlBinaryOperator.NotContains:
                    if (isNullValue)
                    {
                        _sql.Append("1 = 1");
                        return true;
                    }
                    if (node.Right is SqlValue)
                    {
                        var value = ((SqlValue)node.Right).Value.ToString();
                        var comma = new[] { ',', '，' };
                        if (value.IndexOfAny(comma) != -1)
                        {
                            _sql.Append("(");
                            var values = value.Split(comma);
                            Visit(node.Left);
                            if (value.IndexOfAny(new[] { '_', '%' }) == -1)
                            {
                                _sql.Append(" ");
                                if (op == SqlBinaryOperator.NotContains)
                                    _sql.Append("!");
                                _sql.Append("= ");
                            }
                            else
                                VisitBinaryOperator(node.Operator);
                            Visit(node.Right);
                            foreach (var v in values)
                            {
                                _sql.Append(" OR ");
                                Visit(node.Left);
                                if (v.IndexOfAny(new[] { '_', '%' }) == -1)
                                {
                                    _sql.Append(" ");
                                    if (op == SqlBinaryOperator.NotContains)
                                        _sql.Append("!");
                                    _sql.Append("= ");
                                }
                                else
                                    VisitBinaryOperator(node.Operator);
                                Visit(new SqlValue { Value = v });
                            }
                            _sql.Append(")");
                            return true;
                        }
                        if (value.IndexOfAny(new[] { '_', '%' }) == -1)//没有模糊查询，生成=提高效率
                        {
                            Visit(node.Left);
                            _sql.Append(" ");
                            if (op == SqlBinaryOperator.NotContains)
                                _sql.Append("!");
                            _sql.Append("= ");
                            Visit(node.Right);
                            return true;
                        }
                    }
                    break;
                case SqlBinaryOperator.Like:
                case SqlBinaryOperator.StartsWith:
                case SqlBinaryOperator.EndsWith:
                    //如果是空字符串的模糊对比操作，直接认为是真。
                    if (isNullValue)
                    {
                        _sql.Append("1 = 1");
                        return true;
                    }
                    break;
                case SqlBinaryOperator.NotLike:
                case SqlBinaryOperator.NotStartsWith:
                case SqlBinaryOperator.NotEndsWith:
                    //如果是空字符串的模糊对比操作，直接认为是假。
                    if (isNullValue)
                    {
                        _sql.Append("1 != 1");
                        return true;
                    }
                    break;
                case SqlBinaryOperator.In:
                case SqlBinaryOperator.NotIn:
                    //对于 In、NotIn 操作，如果传入的是空列表时，需要特殊处理: In(Empty) 表示 false，NotIn(Empty) 表示 true。
                    if (IsEmptyArray(node.Right))
                    {
                        _sql.Append(op == SqlBinaryOperator.In ? "0 = 1" : "1 = 1");
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        protected override SqlBinaryConstraint VisitSqlBinaryConstraint(SqlBinaryConstraint node)
        {
            //特殊处理
            if (VisitSpecialBinary(node)) return node;

            var op = node.Operator;
            var isNullValue = IsNullValue(node.Right);

            var columnAlias = _columnAlias;
            _columnAlias = false;
            var useParameter = _useParameter;
            try
            {
                Visit(node.Left);
                if (isNullValue)
                {
                    switch (op)
                    {
                        case SqlBinaryOperator.Equal: _sql.Append(" IS NULL"); return node;
                        case SqlBinaryOperator.NotEqual: _sql.Append(" IS NOT NULL"); return node;
                    }
                }

                VisitBinaryOperator(node.Operator);

                if (node.Right is SqlValue)
                {
                    var value = ((SqlValue)node.Right).Value;
                    switch (op)
                    {
                        case SqlBinaryOperator.StartsWith:
                        case SqlBinaryOperator.NotStartsWith:
                            _sql.AppendParameter(Escape(value) + WILDCARD_ALL);
                            AppendEscapePlause(value);
                            return node;
                        case SqlBinaryOperator.EndsWith:
                        case SqlBinaryOperator.NotEndsWith:
                            _sql.AppendParameter(WILDCARD_ALL + Escape(value));
                            AppendEscapePlause(value);
                            return node;
                    }
                }

                if (op == SqlBinaryOperator.In || op == SqlBinaryOperator.NotIn)
                {
                    _sql.Append("(");
                    if (node.Right is SqlArray)
                    {
                        if (((SqlArray)node.Right).Items.Count > MaxItemsInInClause)
                            throw new TooManyParameterException("在 In 语句中使用了过多的参数");
                        _useParameter = false;
                    }
                    else
                    {
                        _sql.AppendLine();
                        Indent++;
                    }
                }

                Visit(node.Right);

                if (op == SqlBinaryOperator.In || op == SqlBinaryOperator.NotIn)
                {
                    if (!(node.Right is SqlArray))
                    {
                        _sql.AppendLine();
                        Indent--;
                    }
                    _sql.Append(")");
                }
            }
            finally
            {
                _columnAlias = columnAlias;
                _useParameter = useParameter;
            }
            return node;
        }

        /// <summary>
        /// 是否空值
        /// </summary>
        protected virtual bool IsNullValue(SqlNode node)
        {
            var v = node as SqlValue;
            if (v == null) return false;
            if (v.Value == null || v.Value == DBNull.Value) return true;
            if (v.Value is string && string.Empty.Equals(v.Value)) return true;
            return false;
        }

        /// <summary>
        /// 是否SqlArray且没有元素
        /// </summary>
        protected virtual bool IsEmptyArray(SqlNode node)
        {
            var v = node as SqlArray;
            return v != null && v.Items.Count == 0;
        }

        string Escape(object value)
        {
            return value.ToString()
                .Replace(WILDCARD_ALL, WILDCARD_ALL_ESCAPED)
                .Replace(WILDCARD_SINGLE, WILDCARD_SINGLE_ESCAPED);
        }

        void AppendEscapePlause(object value)
        {
            //http://blog.sina.com.cn/s/blog_415bd707010006qv.html
            var strValue = value as string;
            if (!string.IsNullOrWhiteSpace(strValue) &&
                (strValue.Contains(WILDCARD_ALL) || strValue.Contains(WILDCARD_SINGLE))
                )
            {
                _sql.Append(" ESCAPE '").Append(ESCAPE_CHAR).Append('\'');
            }
        }

        protected override SqlSelectAll VisitSqlSelectAll(SqlSelectAll sqlSelectStar)
        {
            if (sqlSelectStar.Table != null)
            {
                QuoteAppend(sqlSelectStar.Table.GetName());
                _sql.Append(".*");
            }
            else
                _sql.Append("*");

            return sqlSelectStar;
        }

        protected override SqlExistsConstraint VisitSqlExistsConstraint(SqlExistsConstraint sqlExistsConstraint)
        {
            if (sqlExistsConstraint.IsNot)
                _sql.Append("NOT ");
            _sql.Append("EXISTS (");
            _sql.AppendLine();

            Indent++;
            Visit(sqlExistsConstraint.Select);
            Indent--;

            _sql.AppendLine();
            _sql.Append(")");

            return sqlExistsConstraint;
        }

        protected override SqlNotConstraint VisitSqlNotConstraint(SqlNotConstraint sqlNotConstraint)
        {
            _sql.Append("NOT (");
            Visit(sqlNotConstraint.Constraint);
            _sql.Append(")");

            return sqlNotConstraint;
        }

        protected override SqlSubSelect VisitSqlSubSelect(SqlSubSelect sqlSelectRef)
        {
            _sql.Append("(");
            _sql.AppendLine();
            Indent++;
            Visit(sqlSelectRef.Select);
            Indent--;
            _sql.AppendLine();
            _sql.Append(")");
            AppendNameCast();
            _sql.Append(sqlSelectRef.Alias);

            return sqlSelectRef;
        }

        protected override SqlOrderBy VisitSqlOrderBy(SqlOrderBy sqlOrderBy)
        {
            AppendColumnUsage(sqlOrderBy.Column);
            _sql.Append(" ");
            _sql.Append(sqlOrderBy.Direction == OrderDirection.Ascending ? "ASC" : "DESC");

            return sqlOrderBy;
        }

        protected override SqlOrderByList VisitSqlOrderByList(SqlOrderByList sqlOrderByList)
        {
            if (sqlOrderByList.Count > 0)
            {
                _sql.Append("ORDER BY ");

                for (int i = 0, c = sqlOrderByList.Items.Count; i < c; i++)
                {
                    if (i > 0)
                    {
                        _sql.Append(", ");
                    }
                    Visit(sqlOrderByList.Items[i] as SqlOrderBy);
                }
            }

            return sqlOrderByList;
        }

        protected override SqlGroupBy VisitSqlGroupBy(SqlGroupBy sqlGroupBy)
        {
            AppendColumnUsage(sqlGroupBy.Column);

            return sqlGroupBy;
        }

        protected override SqlGroupByList VisitSqlGroupByList(SqlGroupByList sqlGroupByList)
        {
            if (sqlGroupByList.Count > 0)
            {
                _sql.Append("GROUP BY ");

                for (int i = 0, c = sqlGroupByList.Items.Count; i < c; i++)
                {
                    if (i > 0)
                    {
                        _sql.Append(", ");
                    }
                    Visit(sqlGroupByList.Items[i] as SqlGroupBy);
                }
            }
            return sqlGroupByList;
        }

        protected virtual string VisitFunctionName(string function)
        {
            return function;
        }

        protected override SqlBinaryFunction VisitSqlBinaryFunction(SqlBinaryFunction node)
        {
            _sql.Append("(");
            bool columnAlias = _columnAlias;
            _columnAlias = false;//不需要别名
            bool useParameter = _useParameter;
            _useParameter = false;
            Visit(node.Left);
            _sql.Append(VisitFunctionName(node.Function));
            Visit(node.Right);
            _useParameter = useParameter;
            _sql.Append(")");
            _columnAlias = columnAlias;
            if (_columnAlias && node.Alias.IsNotEmpty())
            {
                AppendNameCast();
                QuoteAppend(node.Alias);
            }
            return node;
        }

        protected override SqlFunction VisitSqlFunction(SqlFunction node)
        {
            _sql.Append(VisitFunctionName(node.Function));
            _sql.Append("(");
            bool columnAlias = _columnAlias;
            _columnAlias = false;//不需要别名
            bool useParameter = _useParameter;
            _useParameter = false;
            Visit(node.Args);
            _useParameter = useParameter;
            _sql.Append(")");
            _columnAlias = columnAlias;
            if (_columnAlias && node.Alias.IsNotEmpty())
            {
                AppendNameCast();
                QuoteAppend(node.Alias);
            }
            return node;
        }

        protected override SqlValue VisitSqlValue(SqlValue node)
        {
            var value = node.Value;
            if (_useParameter)
                _sql.AppendParameter(SqlDialect.PrepareValue(value));
            else
            {
                if (value is string || value is DateTime || value is Guid)
                    _sql.Append('\'').Append(value).Append('\'');
                else
                    _sql.Append(value);
            }
            return node;
        }

        /// <summary>
        /// 把标识符添加到 Sql 语句中。
        /// 子类可重写此方法来为每一个标识符添加引用符。
        /// SqlServer 生成 [identifier]
        /// Oracle 生成 "IDENTIFIER"
        /// </summary>
        /// <param name="identifier"></param>
        protected virtual void QuoteAppend(string identifier)
        {
            identifier = SqlDialect.PrepareIdentifier(identifier);
            _sql.Append(identifier);
        }

        void AppendColumnUsage(SqlColumn sqlColumn)
        {
            var table = sqlColumn.Table;
            if (table != null)
            {
                QuoteAppend(table.GetName());
                _sql.Append(".");
            }
            QuoteAppend(sqlColumn.ColumnName);
        }

        protected virtual void AppendNameCast()
        {
            _sql.Append(" ");
        }

        public abstract ISqlDialect SqlDialect { get; }
    }
}
