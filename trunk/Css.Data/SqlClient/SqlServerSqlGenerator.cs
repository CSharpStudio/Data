using Css.Data.Common;
using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlClient
{
    class SqlServerSqlGenerator : SqlGenerator
    {
        protected override void QuoteAppend(string identifier)
        {
            if (AutoQuota)
            {
                if (!(identifier.StartsWith("[") && identifier.EndsWith("]")) && !(identifier.StartsWith("(") && identifier.EndsWith(")")))
                {
                    identifier = SqlDialect.PrepareIdentifier(identifier);
                }
                Sql.Append(identifier);
            }
            else
            {
                base.QuoteAppend(identifier);
            }
        }

        protected override void AppendNameCast()
        {
            Sql.Append(" AS ");
        }

        /// <summary>
        /// 为指定的原始查询生成指定分页效果的新查询。
        /// </summary>
        /// <param name="raw">原始查询</param>
        /// <param name="pagingInfo">分页信息。</param>
        /// <returns></returns>
        protected override ISqlSelect ModifyToPagingTree(SqlSelect raw, int startRow, int endRow)
        {
            if (!raw.HasOrdered()) { throw new InvalidProgramException("必须排序后才能使用分页功能。"); }

            //如果是第一页，则只需要使用 TOP 语句即可。
            if (startRow == 0)
            {
                return new SqlSelect
                {
                    Selection = new SqlNodeList
                    {
                        new SqlLiteral { FormattedSql = "TOP " + endRow + " " },
                        raw.Selection ?? SqlSelectAll.Default
                    },
                    From = raw.From,
                    Where = raw.Where,
                    OrderBy = raw.OrderBy
                };
            }

            /*********************** 代码块解释 *********************************
             * 
             * 转换方案：
             * 
             * SELECT * 
             * FROM ASN
             * WHERE ASN.Id > 0
             * ORDER BY ASN.AsnCode ASC
             * 
             * 转换分页后：
             * 
             * SELECT TOP 10 * 
             * FROM ASN
             * WHERE ASN.Id > 0 AND ASN.Id NOT IN(
             *     SELECT TOP 20 Id
             *     FROM ASN
             *     WHERE ASN.Id > 0 
             *     ORDER BY ASN.AsnCode ASC
             * )
             * ORDER BY ASN.AsnCode ASC
             * 
            **********************************************************************/

            //先要找到主表的 PK，分页时需要使用此主键列来生成分页 Sql。
            //这里约定 Id 为主键列名。
            var finder = new FirstTableFinder();
            var pkTable = finder.Find(raw.From);
            var pkColumn = new SqlColumn { Table = pkTable, ColumnName = "Id" };

            //先生成内部的 Select
            var excludeSelect = new SqlSelect
            {
                Selection = new SqlNodeList
                {
                    new SqlLiteral { FormattedSql = "TOP " + startRow + " " },
                    pkColumn
                },
                From = raw.From,
                Where = raw.Where,
                OrderBy = raw.OrderBy,
            };

            var res = new SqlSelect
            {
                Selection = new SqlNodeList
                {
                    new SqlLiteral { FormattedSql = "TOP " + (endRow-startRow) + " " },
                    raw.Selection ?? SqlSelectAll.Default
                },
                From = raw.From,
                OrderBy = raw.OrderBy,
            };

            var newWhere = new SqlBinaryConstraint
            {
                Left = pkColumn,
                Operator = BinaryOp.NotIn,
                Right = excludeSelect
            };
            if (raw.Where != null)
            {
                res.Where = new SqlGroupConstraint
                {
                    Left = raw.Where,
                    Opeartor = GroupOp.And,
                    Right = newWhere
                };
            }
            else
            {
                res.Where = newWhere;
            }

            return res;
        }

        protected override string VisitFunctionName(string function)
        {
            switch (function)
            {
                case SqlFunctions.SUBSTR: return "SUBSTRING";
                case SqlFunctions.LENGTH: return "LEN";
                case SqlFunctions.NVL: return "ISNULL";
            }
            return base.VisitFunctionName(function);
        }

        public override ISqlDialect SqlDialect
        {
            get { return DbProvider.GetDialect(DbProvider.SqlClient); }
        }
    }
}
