//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Css.Test
//{
//    class MsSqlDbAccesserTest : DbAccesserTest<MsSqlTestConfig> { }
//    class OracleDbAccesserTest : DbAccesserTest<MsSqlTestConfig> { }

//    abstract class DbAccesserTest<T> : DbTest<T> where T : DbTestConfig
//    {
//        [Test]
//        public void ExecuteNonQuery()
//        {
//            var sql = "INSERT INTO UT_TABLE(ID,NAME,FLAG) VALUES({0}p0,{0}p1,{0}p2)".FormatArgs(Config.ParameterPrefix);
//            var result = DBA.ExecuteNonQuery("INSERT INTO UT_TABLE(ID,NAME,FLAG) VALUES({0},{1},{2})", 1.1, "TestName", true);
//            Assert.AreEqual(1, result);
//            AssertSql(sql);

//            var dialect = DbProvider.GetDialect(Config.ProviderName);
//            var p0 = DBA.CreateParameter("p0", 1.2);
//            var p1 = DBA.CreateParameter("p1", "TestName");
//            var p2 = DBA.CreateParameter("p2", dialect.PrepareValue(true));//Oracle不支持DbType.Boolean,需要用方言处理
//            var rawResult = DBA.RawAccesser.ExecuteNonQuery(sql, System.Data.CommandType.Text, p0, p1, p2);
//            Assert.AreEqual(1, rawResult);
//            AssertSql(sql);
//        }
//        [Test]
//        public void ExecuteReader()
//        {
//            var sql = "SELECT ID,NAME FROM UT_TABLE";
//            using (var reader = DBA.ExecuteReader(sql))
//            {
//                reader.Read();
//                var id = reader.GetDouble("ID");
//                var name = reader.GetString("NAME");
//                Assert.Greater(id, 0);
//                Assert.IsNotEmpty(name);
//                AssertSql(sql);
//            }
//            using (var reader = DBA.RawAccesser.ExecuteReader(sql, System.Data.CommandType.Text, false))
//            {
//                reader.Read();
//                var id = reader.GetDouble("ID");
//                var name = reader.GetString("NAME");
//                Assert.Greater(id, 0);
//                Assert.IsNotEmpty(name);
//                AssertSql(sql);
//            }
//        }
//        [Test]
//        public void ExecuteDataTable()
//        {
//            var sql = "SELECT ID,NAME FROM UT_TABLE";
//            using (var dt = DBA.ExecuteDataTable(sql))
//            {
//                Assert.Greater(dt.Rows.Count, 0);
//                Assert.IsNotEmpty(dt.Rows[0][1] as string);
//                AssertSql(sql);
//            }
//            using (var dt = DBA.RawAccesser.ExecuteDataTable(sql, System.Data.CommandType.Text))
//            {
//                Assert.Greater(dt.Rows.Count, 0);
//                Assert.IsNotEmpty(dt.Rows[0][1] as string);
//                AssertSql(sql);
//            }
//        }
//        [Test]
//        public void ExecuteScalar()
//        {
//            var sql = "SELECT ID,NAME FROM UT_TABLE";
//            var value = DBA.ExecuteScalar(sql);
//            Assert.IsNotNull(value);
//            AssertSql(sql);
//            var rawValue = DBA.ExecuteScalar(sql);
//            Assert.IsNotNull(rawValue);
//            AssertSql(sql);
//        }
//        [Test]
//        public void CreateCommand()
//        {
//            var sql = "SELECT ID,NAME FROM UT_TABLE";
//            using (var cmd = DBA.CreateCommand(sql, System.Data.CommandType.Text))
//            {
//                if (DBA.Connection.State != System.Data.ConnectionState.Open)
//                    DBA.Connection.Open();
//                var value = cmd.ExecuteScalar();
//                Assert.IsNotNull(value);
//                AssertSql(sql);
//            }
//        }
//        [Test]
//        public void CreateParemater()
//        {
//            var p = DBA.CreateParameter("myParam", 3);
//            var sql = "SELECT ID,NAME FROM UT_TABLE WHERE ID={0}myParam".FormatArgs(Config.ParameterPrefix);
//            var value = DBA.RawAccesser.ExecuteScalar(sql, System.Data.CommandType.Text, p);
//            Assert.AreEqual("myParam", p.ParameterName);
//            Assert.AreEqual(3, p.Value);
//            AssertSql(sql);
//        }
//        [Test]
//        public void FormattedSql()
//        {
//            var value = DBA.ExecuteScalar("SELECT ID,NAME FROM UT_TABLE WHERE ID={0}", 3);
//            Assert.AreEqual(3, value);
//            AssertSql("SELECT ID,NAME FROM UT_TABLE WHERE ID={0}p0".FormatArgs(Config.ParameterPrefix));
//        }
//        [Test]
//        public void ExecuteProcedure()
//        {
//            try
//            {
//                Config.CreateTestStoredProcedures();
//                OnSetUp();
//                var p1 = DBA.CreateParameter("param1", 1);
//                var p2 = DBA.CreateParameter("param2", 2);
//                var p3 = DBA.CreateParameter("ret_value", null, System.Data.DbType.Object, System.Data.ParameterDirection.Output);
//                var dt = DBA.RawAccesser.ExecuteDataTable("UT_PKG.UT_PROCEDURE", System.Data.CommandType.StoredProcedure, p1, p2, p3);
//                Assert.AreEqual(1, dt.Rows.Count);
//            }
//            finally
//            {
//                Config.DropTestStoredProcedures();
//            }
//        }
//    }
//}
