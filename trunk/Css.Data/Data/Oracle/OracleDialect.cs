using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Oracle
{
    class OracleDialect : SqlDialect
    {
        public override string ToSpecialDbSql(string commonSql)
        {
            return ReParameterName.Replace(commonSql, ":p${number}");
        }

        public override string GetParameterName(int number)
        {
            return ":p" + number;
        }

        public override void PrepareParameter(IDbDataParameter p)
        {
            if (p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.ReturnValue)
            {
                if (p.DbType == DbType.Object)
                {
                    dynamic param = p;
                    param.OracleDbType = 121;
                    //p.GetType().GetProperty("OracleDbType").SetValue(p, 121);
                    //((OracleParameter)p).OracleDbType = OracleDbType.RefCursor;
                }
            }
        }

        public override object PrepareValue(object value)
        {
            value = value ?? DBNull.Value;
            if (value != DBNull.Value)
            {
                if (value is bool)
                {
                    value = OracleDbTypeHelper.ToDbBoolean((bool)value);
                }
                else if (value.GetType().IsEnum)
                {
                    value = value.ConvertTo<int>();
                }
            }

            return value;
        }

        public override void PrepareCommand(IDbCommand command)
        {
            if (command.CommandType == CommandType.StoredProcedure)
            {
                //TODO加入游标参数
            }
        }

        public override string PrepareIdentifier(string identifier)
        {
            return "\"{0}\"".FormatArgs(LimitOracleIdentifier(identifier.ToUpper()));
        }

        /// <summary>
        /// Oracle 的标识符都不能超过 30 个字符。这个方法可以把传入的字符串剪裁到 30 个字符，并尽量保持信息。
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string LimitOracleIdentifier(string identifier)
        {
            var result = identifier;
            if (identifier.Length > 30)
            {
                if (identifier.StartsWith("SQL_") || identifier.StartsWith("PK_"))
                {
                    var firstIndex = identifier.IndexOf('_');
                    var lastIndex = identifier.LastIndexOf('_');
                    List<char> chars = new List<char>(identifier.Length);
                    for (int i = 0; i < identifier.Length; i++)
                    {
                        if (i != firstIndex && i != lastIndex && identifier[i] != '_')
                            chars.Add(identifier[i]);
                    }
                    result = new string(chars.ToArray());
                }
                else
                {
                    result = identifier.Replace("_", "");
                }
                if (result.Length > 30)
                {
                    //中间5位散列值
                    var hast = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(identifier));
                    var hastString = BitConverter.ToString(hast).Replace("-", "");
                    result = result.Substring(0, 10) + hastString.Substring(0, 5) + result.Substring(result.Length - 15, 15);
                }
            }
            return result;
        }

        public override string DbTimeValueSql()
        {
            return "SYSDATE";
        }

        public override string SelectDbTimeSql()
        {
            return "SELECT SYSDATE FROM DUAL";
        }

        /// <summary>
        /// 获取序列下一个值(Oracle,SqlServer2012)
        /// </summary>
        /// <returns></returns>
        public override string SelectSeqNextValueSql(string tableName, string columnName)
        {
            var name = "SEQ_{0}_{1}".FormatArgs(tableName, columnName);
            return "SELECT {0}.NEXTVAL FROM DUAL".FormatArgs(PrepareIdentifier(name));
        }

        public override string SeqNextValueSql(string tableName, string columnName)
        {
            var name = "SEQ_{0}_{1}".FormatArgs(tableName, columnName);
            return "{0}.NEXTVAL".FormatArgs(PrepareIdentifier(name));
        }
    }
}

