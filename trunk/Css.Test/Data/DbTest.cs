using Css.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Test
{
    abstract class DbTest<T> where T : DbTestConfig
    {
        protected DbTestConfig Config { get; }

        public DbTest()
        {
            Config = Activator.CreateInstance<T>();
        }

        IDbAccesser _dba;
        protected IDbAccesser DBA { get { return _dba ?? (_dba = new DbAccesser(Config.ConnectionString, Config.ProviderName)); } }
        protected string LastSql { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            OnOneTimeSetUp();
        }

        protected virtual void OnOneTimeSetUp()
        {
            Config.CreateTestTables();
            DbAccesserFactory.DbCommandPrepared += (s, e) => { LastSql += e.DbCommand.CommandText; };
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            OnOneTimeTearDown();
        }

        protected virtual void OnOneTimeTearDown()
        {
            Config.DropTestTables();
        }

        protected virtual void AssertSql(string expected)
        {
            try
            {
                Assert.AreEqual(expected, LastSql);
            }
            finally
            {
                LastSql = null;
            }
        }

        [SetUp]
        public void SetUp()
        {
            OnSetUp();
        }

        protected virtual void OnSetUp()
        {
            LastSql = null;
        }
    }
}
