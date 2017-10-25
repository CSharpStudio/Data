using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlClient
{
    internal class SqlServerDialect : SqlDialect
    {
        public override string ToSpecialDbSql(string commonSql)
        {
            return ReParameterName.Replace(commonSql, "@p${number}");
        }

        public override string GetParameterName(int number)
        {
            return "@p" + number;
        }

        public override void PrepareParameter(System.Data.IDbDataParameter p)
        {
            //if (p.Direction == System.Data.ParameterDirection.Output || p.Direction == System.Data.ParameterDirection.ReturnValue)
            //{
            //    if (p.DbType == DbType.Object)
            //    {
            //        p.DbType = DbType.Int32;
            //    }
            //}
        }

        public override void PrepareCommand(IDbCommand command)
        {
            if (command.CommandType == CommandType.StoredProcedure)
            {
                command.CommandText = PrepareIdentifier(command.CommandText);
            }
        }

        public override object PrepareValue(object value)
        {
            return value ?? DBNull.Value;
        }

        public override string PrepareIdentifier(string identifier)
        {
            return "[{0}]".FormatArgs(identifier);
        }

        public override string DbTimeValueSql()
        {
            return "GETDATE()";
        }

        public override string SelectDbTimeSql()
        {
            return "SELECT GETDATE()";
        }

        /// <summary>
        /// 获取序列下一个值(Oracle,SqlServer2012)
        /// </summary>
        /// <returns></returns>
        public override string SelectSeqNextValueSql(string tableName, string columnName)
        {
            return "SELECT NEXT VALUE FOR SEQ_{0}_{1}".FormatArgs(tableName, columnName);
        }

        public override string SeqNextValueSql(string tableName, string columnName)
        {
            return "NEXT VALUE FOR SEQ_{0}_{1}".FormatArgs(tableName, columnName);
        }
    }
}
