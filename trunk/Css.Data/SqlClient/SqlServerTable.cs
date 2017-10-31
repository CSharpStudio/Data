using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlClient
{
    public class SqlServerTable : DbTable
    {
        public SqlServerTable(ITableInfo info) : base(info)
        {
        }

        public override ISqlDialect SqlDialect
        {
            get { return DbProvider.GetDialect(DbProvider.SqlClient); }
        }

        public override SqlGenerator CreateSqlGenerator()
        {
            var generator = new SqlServerSqlGenerator();
            return generator;
        }

        string _insertSql;
        public override void Insert(IDbAccesser dba, object item)
        {
            //如果有 Id 列，那么需要在执行 Insert 的同时，执行 SELECT @@IDENTITY。
            //在为 SQL Server 插入数据时，执行 Insert 的同时，必须同时执行 SELECT @@IDENTITY。否则会有多线程问题。
            if (PKColumn.Info.IsIdentity)
            {
                if (_insertSql == null)
                {
                    _insertSql = GenerateInsertSql();
                    _insertSql += System.Environment.NewLine;
                    _insertSql += "SELECT @@IDENTITY;";
                }

                var parameters = new List<object>(Columns.Count);
                foreach (var column in Columns)
                {
                    if (column.CanInsert)
                    {
                        var value = column.GetValue(item);
                        parameters.Add(value);
                    }
                }

                //由于默认是 decimal 类型，所以需要类型转换。
                var idValue = dba.ExecuteScalar(_insertSql, parameters.ToArray());
                PKColumn.SetValue(item, idValue.ConvertTo(PKColumn.Info.DataType));

                //如果实体的 Id 是在插入的过程中生成的，
                //那么需要在插入组合子对象前，先把新生成的父对象 Id 都同步到子列表中。
                //if (item is Entity)
                //    ((Entity)item).SyncIdToChildren();
            }
            else
            {
                base.Insert(dba, item);
            }
        }
    }
}
