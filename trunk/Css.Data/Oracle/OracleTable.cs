using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Oracle
{
    public class OracleTable : DbTable
    {
        /// <summary>
        /// ORACLE 中 IN 语句的最大参数个数是 1000 个。
        /// </summary>
        const int MAX_ITEMS_IN_INCLAUSE = 1000;

        public OracleTable(ITableInfo info) : base(info)
        {
        }

        public override ISqlDialect SqlDialect
        {
            get { return DbProvider.GetDialect(DbProvider.Oracle); }
        }

        public override SqlGenerator CreateSqlGenerator()
        {
            var generator = new OracleSqlGenerator();
            generator.MaxItemsInInClause = MAX_ITEMS_IN_INCLAUSE;
            return generator;
        }

        protected override DbColumn CreateColumn(IColumnInfo columnInfo)
        {
            return new OracleColumn(this, columnInfo);
        }

        public override void Insert(IDbAccesser dba, object item)
        {
            if (PKColumn.Info.IsIdentity)
            {
                var seq = dba.ExecuteScalar(SqlDialect.SelectSeqNextValueSql(Name, PKColumn.Name));
                PKColumn.SetValue(item, seq.ConvertTo(PKColumn.Info.DataType));

                //如果实体的 Id 是在插入的过程中生成的，
                //那么需要在插入组合子对象前，先把新生成的父对象 Id 都同步到子列表中。
                //if (item is Entity)
                //    ((Entity)item).SyncIdToChildren();
            }

            base.Insert(dba, item);
        }
    }
}
