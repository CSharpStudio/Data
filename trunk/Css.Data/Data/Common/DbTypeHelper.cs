using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    public static class DbTypeHelper
    {
        public static DbType ConvertFromCLRType(Type clrType)
        {
            if (clrType.IsEnum) { return DbType.Int32; }
            if (clrType == typeof(string)) { return DbType.String; }
            if (clrType == typeof(int)) { return DbType.Int32; }
            if (clrType == typeof(long)) { return DbType.Int64; }
            if (clrType == typeof(bool)) { return DbType.Boolean; }
            if (clrType == typeof(DateTime)) { return DbType.DateTime; }
            if (clrType == typeof(Guid)) { return DbType.Guid; }
            if (clrType == typeof(double)) { return DbType.Double; }
            if (clrType == typeof(byte)) { return DbType.Byte; }
            if (clrType == typeof(short)) { return DbType.Int16; }
            if (clrType == typeof(char)) { return DbType.StringFixedLength; }
            if (clrType == typeof(decimal)) { return DbType.Decimal; }
            if (clrType == typeof(float)) { return DbType.Single; }
            if (clrType == typeof(uint)) { return DbType.UInt32; }
            if (clrType == typeof(ulong)) { return DbType.UInt64; }
            if (clrType == typeof(ushort)) { return DbType.UInt16; }
            if (clrType == typeof(sbyte)) { return DbType.SByte; }
            if (clrType == typeof(float)) { return DbType.Single; }
            if (clrType == typeof(byte[])) { return DbType.Binary; }

            if (clrType.IsNullable())
            {
                return ConvertFromCLRType(clrType.IgnoreNullable());
            }

            return DbType.String;
        }

        /// <summary>
        /// 获取type的默认值sql表达式
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetDefaultValue(DbType type)
        {
            switch (type)
            {
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                    return string.Empty;
                case DbType.DateTime:
                    return new DateTime(2000, 1, 1, 0, 0, 0);
                case DbType.Guid:
                    return Guid.Empty.ToString();
                case DbType.Int32:
                case DbType.Int64:
                case DbType.Binary:
                case DbType.Double:
                case DbType.Boolean:
                case DbType.Byte:
                case DbType.Decimal:
                    return 0;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 由于不同的 DbType 映射到库中后的类型可能是相同的，所以这里需要对类型进行兼容性判断。
        /// </summary>
        /// <param name="oldColumnType"></param>
        /// <param name="newColumnType"></param>
        /// <returns></returns>
        internal static bool IsCompatible(DbType oldColumnType, DbType newColumnType)
        {
            if (oldColumnType == newColumnType) return true;

            //如果两个列都属性同一类型的数据库类型，这里也表示库的类型没有变化。
            for (int i = 0, c = CompatibleTypes.Length; i < c; i++)
            {
                var sameTypes = CompatibleTypes[i];
                if (sameTypes.Contains(oldColumnType) && sameTypes.Contains(newColumnType))
                {
                    return true;
                }
            }

            return false;
        }
        static DbType[][] CompatibleTypes = new DbType[][]{
            new DbType[]{ DbType.String, DbType.AnsiString, DbType.Xml },
            new DbType[]{ DbType.Int64, DbType.Double, DbType.Decimal },
        };
    }
}
