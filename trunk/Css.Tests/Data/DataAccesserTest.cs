using Css.ComponentModel;
using Css.Data;
using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Css.Tests.Data
{
    public class SqlServerDataAccessTest : DataAccesserTest<SqlServerFixture>
    {
        public SqlServerDataAccessTest(SqlServerFixture fixture) : base(fixture) { }
    }
    public class OracleDataAccessTest : DataAccesserTest<OracleFixture>
    {
        public OracleDataAccessTest(OracleFixture fixture) : base(fixture) { }
    }

    public abstract class DataAccesserTest<T> : DisposableBase, IClassFixture<T> where T : DbFixture
    {
        T _dbFixture;
        IDbAccesser _dba;
        string _commandText;
        public DataAccesserTest(T fix)
        {
            _dbFixture = fix;
            _dba = new DbAccesser(_dbFixture.ConnectionString, _dbFixture.ProviderName);
            DbAccesserFactory.DbCommandPrepared += DbAccesserFactory_DbCommandPrepared;
        }

        private void DbAccesserFactory_DbCommandPrepared(object sender, DbCommandEventArgs e)
        {
            _commandText += e.DbCommand.CommandText;
        }

        void AssertSql(string sql)
        {
            Assert.Equal(_commandText, sql);
            _commandText = null;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            _dba.Dispose();
            DbAccesserFactory.DbCommandPrepared -= DbAccesserFactory_DbCommandPrepared;
        }

        [Fact]
        public void ExecuteNonQueryWithFormattedSql()
        {
            var sql = "INSERT INTO UT_TABLE(ID,NAME,FLAG) VALUES({0}p0,{0}p1,{0}p2)".FormatArgs(_dbFixture.ParameterPrefix);
            var result = _dba.ExecuteNonQuery("INSERT INTO UT_TABLE(ID,NAME,FLAG) VALUES({0},{1},{2})", 1.1, "TestName", true);
            Assert.Equal(1, result);
            AssertSql(sql);
        }
        [Fact]
        public void ExecuteNonQueryWithSpecificSql()
        {
            var sql = "INSERT INTO UT_TABLE(ID,NAME,FLAG) VALUES({0}p0,{0}p1,{0}p2)".FormatArgs(_dbFixture.ParameterPrefix);
            var dialect = DbProvider.GetDialect(_dbFixture.ProviderName);
            var p0 = _dba.CreateParameter("p0", 1.2);
            var p1 = _dba.CreateParameter("p1", "TestName");
            var p2 = _dba.CreateParameter("p2", dialect.PrepareValue(true));//Oracle不支持DbType.Boolean,需要用方言处理
            var rawResult = _dba.ExecuteNonQuery(sql, p0, p1, p2);
            Assert.Equal(1, rawResult);
            AssertSql(sql);
        }
        [Fact]
        public void ExecuteReaderWithFormattedSql()
        {
            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}p0".FormatArgs(_dbFixture.ParameterPrefix);
            using (var reader = _dba.ExecuteReader("SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}", new object[] { 0 }))
            {
                reader.Read();
                var id = reader.GetDouble("ID");
                var name = reader.GetString("NAME");
                Assert.True(id > 0);
                Assert.NotEmpty(name);
                AssertSql(sql);
            }
        }
        [Fact]
        public void ExecuteReaderWithSpecificSql()
        {
            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}p0".FormatArgs(_dbFixture.ParameterPrefix);
            var p0 = _dba.CreateParameter("p0", 0);
            using (var reader = _dba.ExecuteReader(sql, p0))
            {
                reader.Read();
                var id = reader.GetDouble("ID");
                var name = reader.GetString("NAME");
                Assert.True(id > 0);
                Assert.NotEmpty(name);
                AssertSql(sql);
            }
        }
        [Fact]
        public void ExecuteDataTableWithFormattedSql()
        {
            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}p0".FormatArgs(_dbFixture.ParameterPrefix);
            using (var dt = _dba.ExecuteDataTable("SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}", new object[] { 0 }))
            {
                Assert.True(dt.Rows.Count > 0);
                Assert.NotEmpty(dt.Rows[0][1] as string);
                AssertSql(sql);
            }
        }
        [Fact]
        public void ExecuteDataTableWithSpecificSql()
        {
            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}p0".FormatArgs(_dbFixture.ParameterPrefix);
            var p0 = _dba.CreateParameter("p0", 0);
            using (var dt = _dba.ExecuteDataTable(sql, p0))
            {
                Assert.True(dt.Rows.Count > 0);
                Assert.NotEmpty(dt.Rows[0][1] as string);
                AssertSql(sql);
            }
        }
        [Fact]
        public void ExecuteScalarWithFormattedSql()
        {
            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}p0".FormatArgs(_dbFixture.ParameterPrefix);
            var value = _dba.ExecuteScalar("SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}", new object[] { 0 });
            Assert.NotNull(value);
            AssertSql(sql);
        }
        [Fact]
        public void ExecuteScalarWithSpecificSql()
        {
            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID>{0}p0".FormatArgs(_dbFixture.ParameterPrefix);
            var p0 = _dba.CreateParameter("p0", 0);
            var rawValue = _dba.ExecuteScalar(sql, p0);
            Assert.NotNull(rawValue);
            AssertSql(sql);
        }
        [Fact]
        public void CreateCommand()
        {
            var sql = "SELECT ID,NAME FROM UT_TABLE";
            using (var cmd = _dba.CreateCommand(sql, System.Data.CommandType.Text))
            {
                if (_dba.Connection.State != System.Data.ConnectionState.Open)
                    _dba.Connection.Open();
                var value = cmd.ExecuteScalar();
                Assert.NotNull(value);
                AssertSql(sql);
            }
        }
        [Fact]
        public void CreateParemater()
        {
            var p = _dba.CreateParameter("myParam", 3);
            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID={0}myParam".FormatArgs(_dbFixture.ParameterPrefix);
            var value = _dba.ExecuteScalar(sql, System.Data.CommandType.Text, p);
            Assert.Equal("myParam", p.ParameterName);
            Assert.Equal(3, p.Value);
            AssertSql(sql);
        }
        [Fact]
        public void FormattedSql()
        {
            var value = _dba.ExecuteScalar("SELECT ID,NAME FROM UT_TABLE WHERE ID={0} or ID={1} or ID={2}", 1, 2, 3);
            AssertSql("SELECT ID,NAME FROM UT_TABLE WHERE ID={0}p0 or ID={0}p1 or ID={0}p2".FormatArgs(_dbFixture.ParameterPrefix));
        }
        [Fact]
        public void ExecuteProcedure()
        {
            var p1 = _dba.CreateParameter("param1", 1);
            var p2 = _dba.CreateParameter("param2", 2);
            var p3 = _dba.CreateParameter("ret_value", null, System.Data.DbType.Object, System.Data.ParameterDirection.Output);
            var dt = _dba.ExecuteDataTable("UT_PKG.UT_PROCEDURE", System.Data.CommandType.StoredProcedure, p1, p2, p3);
            Assert.Equal(1, dt.Rows.Count);
        }
    }
}
