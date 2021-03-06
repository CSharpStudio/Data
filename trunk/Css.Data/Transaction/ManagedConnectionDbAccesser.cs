﻿using Css.ComponentModel;
using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Transaction
{
    /// <summary>
    /// 使用 ConnectionManager 管理链接的数据库访问器。
    /// </summary>
    internal class ManagedConnectionDbAccesser : DisposableBase, IDbAccesser
    {
        private IConnectionManager _connectionManager;

        private DbAccesser _dba;

        internal ManagedConnectionDbAccesser(IDbSetting dbSetting)
        {
            if (dbSetting == null) throw new ArgumentNullException("dbSetting");

            _connectionManager = TransactionScopeConnectionManager.GetManager(dbSetting);

            _dba = new DbAccesser(dbSetting, _connectionManager.Connection);
        }

        public IDbConnection Connection
        {
            get { return _dba.Connection; }
        }

        public ISqlDialect SqlDialect => _dba.SqlDialect;

        public IDbParameterFactory ParameterFactory => _dba.ParameterFactory;

        public DataTable ExecuteDataTable(string sql, CommandType type, params IDbDataParameter[] parameters)
        {
            return _dba.ExecuteDataTable(sql, type, parameters);
        }

        public int ExecuteNonQuery(string sql, CommandType type, params IDbDataParameter[] parameters)
        {
            return _dba.ExecuteNonQuery(sql, type, parameters);
        }

        public SafeDataReader ExecuteReader(string sql, CommandType type, bool closeConnection, params IDbDataParameter[] parameters)
        {
            return _dba.ExecuteReader(sql, type, closeConnection, parameters);
        }

        public object ExecuteScalar(string sql, CommandType type, params IDbDataParameter[] parameters)
        {
            return _dba.ExecuteScalar(sql, type, parameters);
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            _connectionManager.Dispose();
            _dba.Dispose();
        }

        public IDbCommand CreateCommand(string sql, CommandType type, params IDbDataParameter[] parameters)
        {
            return _dba.CreateCommand(sql, type, parameters);
        }
    }
}
