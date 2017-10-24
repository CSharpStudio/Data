using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.SqlClient
{
    class SqlServerDialect : ISqlDialect
    {
        public string ToSpecialDbSql(string commonSql)
        {
            return SqlDialect.ReParameterName.Replace(commonSql, "@p${number}");
        }

        public string GetParameterName(int number)
        {
            return "@p" + number;
        }

        public void PrepareParameter(IDbDataParameter p)
        {

        }

        public void PrepareCommand(IDbCommand command)
        {

        }

        public object PrepareValue(object value)
        {
            return value ?? DBNull.Value;
        }

        public string PrepareIdentifier(string identifier)
        {
            return "[{0}]".FormatArgs(identifier);
        }

        public string DbTimeValueSql()
        {
            return "GETDATE()";
        }

        public string SelectDbTimeSql()
        {
            return "SELECT GETDATE()";
        }

        /// <summary>
        /// 获取序列下一个值(Oracle,SqlServer2012)
        /// </summary>
        /// <returns></returns>
        public string SelectSeqNextValueSql(string tableName, string columnName)
        {
            return "SELECT NEXT VALUE FOR SEQ_{0}_{1}".FormatArgs(tableName, columnName);
        }

        public string SeqNextValueSql(string tableName, string columnName)
        {
            return "NEXT VALUE FOR SEQ_{0}_{1}".FormatArgs(tableName, columnName);
        }
    }
}
