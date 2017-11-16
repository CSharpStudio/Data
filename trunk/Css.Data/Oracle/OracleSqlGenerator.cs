using Css.Data.Common;
using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Oracle
{
    //暂时没有处理：
    //TOP、!=、
    class OracleSqlGenerator : SqlGenerator
    {
        protected override void QuoteAppend(string identifier)
        {
            if (AutoQuota)
            {
                if (!(identifier.StartsWith("\"") && identifier.EndsWith("\"")) && !(identifier.StartsWith("(") && identifier.EndsWith(")")))
                {
                    identifier = SqlDialect.PrepareIdentifier(identifier);
                    Sql.Append(identifier);
                }
                else
                {
                    Sql.Append(identifier);
                }
            }
            else
            {
                base.QuoteAppend(identifier);
            }
        }

        /// <summary>
        /// 使用 ROWNUM 来进行分页。
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <param name="startRow">start row</param>
        /// <param name="endRow">end row</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">pagingInfo</exception>
        /// <exception cref="System.InvalidProgramException">必须排序后才能使用分页功能。</exception>
        protected override ISqlSelect ModifyToPagingTree(SqlSelect raw, int startRow, int endRow)
        {
            if (!raw.HasOrdered()) { throw new InvalidProgramException("必须排序后才能使用分页功能。"); }

            var res = MakePagingTree(raw, startRow, endRow);

            return res;
        }

        static ISqlSelect MakePagingTree(SqlSelect raw, int startRow, int endRow)
        {
            /*********************** 代码块解释 *********************************
             * 以下转换使用 ORACLE 行号字段来实现分页。只需要简单地在查询的 WHERE 语句中加入等号的判断即可。
             *
             * 源格式：
             *     SELECT *
             *     FROM A
             *     WHERE A.Id > 0
             *     ORDER BY A.NAME ASC
             * 
             * 目标格式：
             *      SELECT * FROM
             *      (
             *          SELECT T.*, ROWNUM RN
             *          FROM (
             *              SELECT *
             *              FROM A
             *              WHERE A.Id > 0
             *              ORDER BY A.NAME ASC
             *          ) T
             *          WHERE ROWNUM <= 20
             *      )
             *      WHERE RN >= 10
            **********************************************************************/

            return new SqlNodeList
            {
                new SqlLiteral(
@"SELECT * FROM
(
    SELECT T.*, ROWNUM RN
    FROM 
    (
"),
                raw,
                new SqlLiteral(
@"
    ) T
    WHERE ROWNUM <= " + endRow + @"
)
WHERE RN >= " + startRow)
            };
        }

        static ISqlSelect MakePagingTree_ReserveMethod(SqlSelect raw, int startRow, int endRow)
        {
            /*********************** 代码块解释 *********************************
             * 源格式：
             *     SELECT *
             *     FROM A
             *     WHERE A.Id > 0
             *     ORDER BY A.NAME ASC
             * 
             * 目标格式：
             *      SELECT * FROM
             *      (SELECT A.*, ROWNUM RN
             *     FROM A
             *     WHERE A.Id > 0 AND ROWNUM <= 20
             *     ORDER BY A.NAME ASC)
             *     WHERE RN >= 10
             *     
             * 这种方法可能存在问题：
             * 因为源 Sql 可能是：Select * From A Join B，这时表示结果集需要显示 A 和 B 的所有字段，
             * 但是此方法会转换为：Select A.* From A Join B。比较麻烦，暂不处理
            **********************************************************************/
            var innerSelect = new SqlSelect
            {
                IsDistinct = raw.IsDistinct,
                From = raw.From,
                Where = AppendWhere(raw.Where, new SqlLiteral("ROWNUM <= " + endRow)),
                OrderBy = raw.OrderBy
            };

            //内部的 Select 子句中，不能简单地使用 "*, ROWNUM"，而是需要使用 "A.*, ROWNUM"
            var rawSelection = raw.Selection;
            if (rawSelection == null)
            {
                //默认约定第一张表，就是
                var table = new FirstTableFinder().Find(raw.From);
                rawSelection = new SqlSelectAll
                {
                    Table = table
                };
            }
            innerSelect.Selection = new SqlNodeList
            {
                rawSelection,
                new SqlLiteral(", ROWNUM RN")
            };

            var res = new SqlSelect
            {
                Selection = SqlSelectAll.Default,
                From = new SqlSubSelect
                {
                    Select = innerSelect
                },
                Where = new SqlLiteral("RN >= " + startRow)
            };
            return res;
        }

        static ISqlConstraint AppendWhere(ISqlConstraint old, ISqlConstraint newConstraint)
        {
            if (old != null)
            {
                newConstraint = new SqlGroupConstraint
                {
                    Left = old,
                    Opeartor = GroupOp.And,
                    Right = newConstraint
                };
            }
            return newConstraint;
        }

        public override ISqlDialect SqlDialect
        {
            get { return DbProvider.GetDialect(DbProvider.Oracle); }
        }
    }
}
