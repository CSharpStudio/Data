using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Css.Data.Common
{
    public abstract class SqlDialect : ISqlDialect
    {
        public abstract string DbTimeValueSql();

        public abstract string GetParameterName(int number);

        public virtual void PrepareCommand(IDbCommand command) { }

        public abstract string PrepareIdentifier(string identifier);

        public virtual void PrepareParameter(IDbDataParameter p) { }

        public virtual object PrepareValue(object value)
        {
            return value ?? DBNull.Value;
        }

        public abstract string SelectDbTimeSql();

        public abstract string SelectSeqNextValueSql(string tableName, string columnName);

        public abstract string SeqNextValueSql(string tableName, string columnName);

        public abstract string ToSpecialDbSql(string commonSql);

        /// <summary>
        /// 在 FormatSQL 中的参数格式定义。
        /// </summary>
        internal static readonly Regex ReParameterName = new Regex(@"{(?<number>\d+)}", RegexOptions.Compiled);
    }
}
