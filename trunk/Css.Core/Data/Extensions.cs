using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Css.Data
{
    public static class Extensions
    {
        /// <summary>
        /// 此方法提供特定数据库的参数列表。
        /// </summary>
        /// <param name="parametersValues">formattedSql参数列表</param>
        /// <returns>数据库参数列表</returns>
        static IDbDataParameter[] ConvertFormatParamaters(IDbAccesser dba, object[] parametersValues)
        {
            IDbDataParameter[] dbParameters = new DbParameter[parametersValues.Length];
            string parameterName = null;
            for (int i = 0, l = parametersValues.Length; i < l; i++)
            {
                parameterName = dba.SqlDialect.GetParameterName(i);
                object value = parametersValues[i];

                //convert null value.
                if (value == null) { value = DBNull.Value; }
                IDbDataParameter param = dba.CreateParameter(parameterName, value, ParameterDirection.Input);
                dbParameters[i] = param;
            }
            return dbParameters;
        }

        /// <summary>
        /// Execute a sql which is not a database procudure, return rows effected.
        /// </summary>
        /// <param name="formattedSql">a formatted sql which format looks like the parameter of String.Format</param>
        /// <param name="parameters">If this sql has some parameters, these are its parameters.</param>
        /// <returns>The number of rows effected.</returns>
        public static int ExecuteNonQuery(this IDbAccesser dba, string formattedSql, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                formattedSql = dba.SqlDialect.ToSpecialDbSql(formattedSql);
                IDbDataParameter[] dbParameters = ConvertFormatParamaters(dba, parameters);
                return dba.ExecuteNonQuery(formattedSql, CommandType.Text, dbParameters);
            }
            return dba.ExecuteNonQuery(formattedSql, CommandType.Text);
        }

        /// <summary>
        /// Execute the sql, and return the element of first row and first column, ignore the other values.
        /// </summary>
        /// <param name="formattedSql">a formatted sql which format looks like the parameter of String.Format</param>
        /// <param name="parameters">If this sql has some parameters, these are its parameters.</param>
        /// <returns>DBNull or value object.</returns>
        public static object ExecuteScalar(this IDbAccesser dba, string formattedSql, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                formattedSql = dba.SqlDialect.ToSpecialDbSql(formattedSql);
                IDbDataParameter[] dbParameters = ConvertFormatParamaters(dba, parameters);
                return dba.ExecuteScalar(formattedSql, CommandType.Text, dbParameters);
            }
            return dba.ExecuteScalar(formattedSql, CommandType.Text);
        }

        /// <summary>
        /// Query out some data from database.
        /// </summary>
        /// <param name="formattedSql">a formatted sql which format looks like the parameter of String.Format</param>
        /// <param name="closeConnection">Indicates whether to close the corresponding connection when the reader is closed?</param>
        /// <param name="parameters">If this sql has some parameters, these are its parameters.</param>
        /// <returns></returns>
        public static SafeDataReader ExecuteReader(this IDbAccesser dba, string formattedSql, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                formattedSql = dba.SqlDialect.ToSpecialDbSql(formattedSql);
                IDbDataParameter[] dbParameters = ConvertFormatParamaters(dba, parameters);
                return dba.ExecuteReader(formattedSql, CommandType.Text, dba.Connection.State == ConnectionState.Closed, dbParameters);
            }
            return dba.ExecuteReader(formattedSql, CommandType.Text, dba.Connection.State == ConnectionState.Closed);
        }

        /// <summary>
        /// Query out a DataTable object from database by the specific sql.
        /// </summary>
        /// <param name="formattedSql">a formatted sql which format looks like the parameter of String.Format</param>
        /// <param name="parameters">If this sql has some parameters, these are its parameters.</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this IDbAccesser dba, string formattedSql, params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                formattedSql = dba.SqlDialect.ToSpecialDbSql(formattedSql);
                IDbDataParameter[] dbParameters = ConvertFormatParamaters(dba, parameters);
                return dba.ExecuteDataTable(formattedSql, CommandType.Text, dbParameters);
            }
            return dba.ExecuteDataTable(formattedSql, CommandType.Text);
        }

        /// <summary>
        /// 执行sql返回受影响行数
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQuery(this IDbAccesser dba, string sql)
        {
            return dba.ExecuteNonQuery(sql, CommandType.Text);
        }

        /// <summary>
        /// 执行sql返回受影响行数
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <param name="parameters">sql所包含的参数</param>
        /// <returns>受影响行数</returns>
        public static int ExecuteNonQuery(this IDbAccesser dba, string sql, params IDbDataParameter[] parameters)
        {
            return dba.ExecuteNonQuery(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 执行sql返回结果集第一行第一列，忽略其它值
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <param name="parameters">sql所包含的参数</param>
        /// <returns><see cref="DBNull"/>或者对象</returns>
        public static object ExecuteScalar(this IDbAccesser dba, string sql)
        {
            return dba.ExecuteScalar(sql, CommandType.Text);
        }

        /// <summary>
        /// 执行sql返回结果集第一行第一列，忽略其它值
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <param name="parameters">sql所包含的参数</param>
        /// <returns><see cref="DBNull"/>或者对象</returns>
        public static object ExecuteScalar(this IDbAccesser dba, string sql, params IDbDataParameter[] parameters)
        {
            return dba.ExecuteScalar(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// 从关系数据库中查询数据
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <param name="closeConnection">数据读取器关闭的时候是否关闭链接</param>
        /// <returns>数据安全读取器</returns>
        public static SafeDataReader ExecuteReader(this IDbAccesser dba, string sql)
        {
            return dba.ExecuteReader(sql, CommandType.Text, dba.Connection.State == ConnectionState.Closed);
        }

        /// <summary>
        /// 从关系数据库中查询数据
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <param name="closeConnection">数据读取器关闭的时候是否关闭链接</param>
        /// <param name="parameters">sql所包含的参数</param>
        /// <returns>数据安全读取器</returns>
        public static SafeDataReader ExecuteReader(this IDbAccesser dba, string sql, params IDbDataParameter[] parameters)
        {
            return dba.ExecuteReader(sql, CommandType.Text, dba.Connection.State == ConnectionState.Closed, parameters);
        }

        /// <summary>
        /// 从关系数据库中查询数据并返回<see cref="DataTable"/>
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(this IDbAccesser dba, string sql)
        {
            return dba.ExecuteDataTable(sql, CommandType.Text);
        }

        /// <summary>
        /// 从关系数据库中查询数据并返回<see cref="DataTable"/>
        /// </summary>
        /// <param name="sql">指定的sql</param>
        /// <param name="parameters">sql所包含的参数</param>
        /// <returns>数据表</returns>
        public static DataTable ExecuteDataTable(this IDbAccesser dba, string sql, params IDbDataParameter[] parameters)
        {
            return dba.ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <inheritdoc cref="IDbParameterFactory.CreateParameter()"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba)
        {
            return dba.ParameterFactory.CreateParameter();
        }
        /// <inheritdoc cref="IDbParameterFactory.CreateParameter(string)"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba, string name)
        {
            return dba.ParameterFactory.CreateParameter(name);
        }
        /// <inheritdoc cref="IDbParameterFactory.CreateParameter(string, object)"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba, string name, object value)
        {
            return dba.ParameterFactory.CreateParameter(name, value);
        }
        /// <inheritdoc cref="IDbParameterFactory.CreateParameter(string, object, DbType)"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba, string name, object value, DbType type)
        {
            return dba.ParameterFactory.CreateParameter(name, value, type);
        }
        /// <inheritdoc cref="IDbParameterFactory.CreateParameter(string, object, ParameterDirection)"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba, string name, object value, ParameterDirection direction)
        {
            return dba.ParameterFactory.CreateParameter(name, value, direction);
        }
        /// <inheritdoc cref="IDbParameterFactory.CreateParameter(string, object, DbType, int)"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba, string name, object value, DbType type, int size)
        {
            return dba.ParameterFactory.CreateParameter(name, value, type, size);
        }
        /// <inheritdoc cref="IDbParameterFactory.CreateParameter(string, object, DbType, ParameterDirection)"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba, string name, object value, DbType type, ParameterDirection direction)
        {
            return dba.ParameterFactory.CreateParameter(name, value, type, direction);
        }
        /// <inheritdoc cref="IDbParameterFactory.CreateParameter(string, object, DbType, int, ParameterDirection)"/>
        public static IDbDataParameter CreateParameter(this IDbAccesser dba, string name, object value, DbType type, int size, ParameterDirection direction)
        {
            return dba.ParameterFactory.CreateParameter(name, value, type, size, direction);
        }
    }
}
