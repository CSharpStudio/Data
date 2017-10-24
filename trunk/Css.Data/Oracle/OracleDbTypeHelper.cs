using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Oracle
{
    internal static class OracleDbTypeHelper
    {
        public static string ToDbBoolean(bool value)
        {
            //数据库使用 CHAR(1) 来存储 Boolean 类型数据。
            return value ? "1" : "0";
        }

        public static bool? ToClrBoolean(object value)
        {
            if (value == null)
                return null;
            return value.ToString() == "1" ? true : false;
        }

        public static DbType ToDbType(Type clrType)
        {
            var value = DbTypeHelper.ToDbType(clrType);
            if (value == DbType.Boolean)
            {
                value = DbType.String;
            }
            return value;
        }

        /// <summary>
        /// 把 DbType 转换为 Oracle 中的数据类型
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static string ToOracleTypeString(DbType fieldType, string length = null)
        {
            switch (fieldType)
            {
                case DbType.String:
                case DbType.AnsiString:
                    if (length.IsNotEmpty())
                    {
                        if ("max".CIEquals(length))
                            return "CLOB";
                        return "VARCHAR2(" + length + ')';
                    }
                    return "VARCHAR2(80)";
                case DbType.Xml:
                    return "XMLTYPE";
                case DbType.DateTime:
                    return "DATE";
                case DbType.Int32:
                    return "INTEGER";
                case DbType.Int64:
                case DbType.Double:
                case DbType.Decimal:
                    return "NUMBER";
                case DbType.Binary:
                    return "BLOB";
                case DbType.Boolean:
                    return "CHAR(1)";
                case DbType.Byte:
                    return "BYTE";
                default:
                    break;
            }
            throw new NotSupportedException(string.Format("不支持生成列类型：{0}。", fieldType));
        }

        /// <summary>
        /// 把 Oracle 中的数据类型 转换为 DbType
        /// </summary>
        /// <param name="lowerSqlType">Type of the lower SQL.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public static DbType ConvertFromOracleTypeString(string lowerSqlType)
        {
            if (lowerSqlType.StartsWith("timestamp"))
                return DbType.DateTime;
            switch (lowerSqlType)
            {
                case "clob":
                case "nvarchar2":
                case "varchar2":
                    return DbType.String;
                case "xmltype":
                    return DbType.Xml;
                case "integer":
                    return DbType.Int32;
                case "number":
                    return DbType.Double;
                case "char":
                    return DbType.Boolean;
                case "blob":
                    return DbType.Binary;
                case "byte":
                    return DbType.Byte;
                case "date":
                    return DbType.DateTime;
                default:
                    break;
            }
            throw new NotSupportedException(string.Format("不支持读取数据库中的列类型：{0}。", lowerSqlType));
        }
    }
}
