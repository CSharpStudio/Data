using Css.ComponentModel;
using Css.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Css.Tests.Data
{
    public class SqlServerTransactionTest : TransactionTest<SqlServerFixture>
    {
        public SqlServerTransactionTest(SqlServerFixture fixture) : base(fixture) { }
    }
    public class OracleTransactionTest : TransactionTest<OracleFixture>
    {
        public OracleTransactionTest(OracleFixture fixture) : base(fixture) { }
    }

    public abstract class TransactionTest<T> : DisposableBase, IClassFixture<T> where T : DbFixture
    {
        T _fixture;
        public TransactionTest(T fixture)
        {
            _fixture = fixture;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
        }

        [Fact]
        public void TransactionScope()
        {
            var setting = DbSetting.SetSetting("UT", _fixture.ConnectionString, _fixture.ProviderName);
            using (var tran = DbAccesserFactory.TransactionScope(setting))
            {
                using (var dba = DbAccesserFactory.Create("UT"))
                {
                    var cmd = dba.CreateCommand("INSERT INTO UT_TABLE(ID, NAME) VALUES(5.1,'TRAN TEST')", System.Data.CommandType.Text);
                    Assert.Same(tran.WholeTransaction, cmd.Transaction);
                    cmd.ExecuteNonQuery();

                    using (var innerTran = DbAccesserFactory.TransactionScope(setting))
                    {
                        Assert.Same(tran.WholeTransaction, innerTran.WholeTransaction);
                        using (var inner = DbAccesserFactory.Create("UT"))
                        {
                            var innerCmd = inner.CreateCommand("INSERT INTO UT_TABLE(ID, NAME) VALUES(5.2,'TRAN TEST')", System.Data.CommandType.Text);
                            Assert.Same(cmd.Transaction, innerCmd.Transaction);
                            innerCmd.ExecuteNonQuery();
                        }
                        innerTran.Complete();
                    }
                }
                tran.Complete();
            }
        }

        [Fact]
        public void AutonomousTransactionScope()
        {
            var setting = DbSetting.SetSetting("UT", _fixture.ConnectionString, _fixture.ProviderName);
            using (var tran = DbAccesserFactory.TransactionScope(setting))
            {
                using (var dba = DbAccesserFactory.Create("UT"))
                {
                    var cmd = dba.CreateCommand("INSERT INTO UT_TABLE(ID, NAME) VALUES(5.3,'TRAN TEST')", System.Data.CommandType.Text);
                    Assert.Same(tran.WholeTransaction, cmd.Transaction);
                    cmd.ExecuteNonQuery();

                    using (var innerTran = DbAccesserFactory.AutonomousTransactionScope(setting))
                    {
                        using (var inner = DbAccesserFactory.Create("UT"))
                        {
                            var innerCmd = inner.CreateCommand("INSERT INTO UT_TABLE(ID, NAME) VALUES(5.4,'TRAN TEST')", System.Data.CommandType.Text);
                            Assert.NotSame(cmd.Transaction, innerCmd.Transaction);
                            innerCmd.ExecuteNonQuery();
                        }
                        Assert.NotSame(tran.WholeTransaction, innerTran.WholeTransaction);
                        innerTran.Complete();
                    }
                }
                tran.Complete();
            }
        }
    }
}
