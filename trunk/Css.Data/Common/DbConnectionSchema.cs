using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Css.Data.Common
{
    /// <summary>
    /// 数据库连接结构/方案
    /// </summary>
    public class DbConnectionSchema
    {
        string _database;

        public DbConnectionSchema(string connectionString, string providerName)
        {
            ConnectionString = connectionString;
            ProviderName = providerName;
        }

        /// <summary>
        /// 子类使用
        /// </summary>
        internal DbConnectionSchema() { }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; internal set; }

        /// <summary>
        /// 连接的提供器名称
        /// </summary>
        public string ProviderName { get; internal set; }

        /// <summary>
        /// 对应的数据库名称
        /// </summary>
        public string Database
        {
            get { return (_database ?? (_database = ParseDbName())); }
        }

        string ParseDbName()
        {
            var con = CreateConnection();
            var database = con.Database;

            //System.Data.OracleClient 解析不出这个值，需要特殊处理。
            if (database.IsNullOrWhiteSpace())
            {
                //Oracle 中，把用户名（Schema）认为数据库名。
                var match = Regex.Match(ConnectionString, @"User Id=\s*(?<dbName>\w+)\s*");
                if (!match.Success)
                {
                    throw new NotSupportedException("无法解析出此数据库连接字符串中的数据库名：" + ConnectionString);
                }
                database = match.Groups["dbName"].Value;
            }

            return database;
        }

        /// <summary>
        /// 使用当前的结构来创建一个连接。
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            var factory = DbProvider.GetFactory(ProviderName);

            var connection = factory.CreateConnection();
            connection.ConnectionString = ConnectionString;

            return connection;
        }
    }
}