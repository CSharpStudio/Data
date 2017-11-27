using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Css.Data.Common
{
    /// <summary>
    /// Sql方言，针对各种关系数据库的差异进行处理的接口
    /// </summary>
    public interface ISqlDialect
    {
        /// <summary>
        /// 把可用于String.Format格式的字符串转换为特定数据库格式的字符串
        /// </summary>
        /// <param name="commonSql">可用于String.Format格式的字符串</param>
        /// <returns>可用于特定数据库的sql语句</returns>
        string ToSpecialDbSql(string commonSql);

        /// <summary>
        /// 获取参数名称
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        string GetParameterName(int number);

        /// <summary>
        /// 处理参数
        /// </summary>
        /// <param name="p"></param>
        void PrepareParameter(IDbDataParameter p);

        /// <summary>
        /// 处理参数值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object PrepareValue(object value);

        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command"></param>
        void PrepareCommand(IDbCommand command);

        /// <summary>
        /// 处理标识符
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string PrepareIdentifier(string identifier);

        /// <summary>
        /// 数据库时间值的SQL
        /// </summary>
        /// <returns></returns>
        string DbTimeValueSql();

        /// <summary>
        /// Select数据库时间值的SQL
        /// </summary>
        /// <returns></returns>
        string SelectDbTimeSql();

        /// <summary>
        /// 获取序列下一个值(Oracle,SqlServer2012)
        /// </summary>
        /// <returns></returns>
        string SelectSeqNextValueSql(string tableName, string columnName);

        string SeqNextValueSql(string tableName, string columnName);
    }
}
