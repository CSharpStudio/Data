using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Tests.Data
{
    /// <summary>
    /// 数据库测试
    /// </summary>
    public abstract class DbFixture : DisposableBase
    {
        public const string SqlServerConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DbMaster;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public const string OracleConnection = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.175.70)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = MESDEV)));User Id=MESDEV;Password=MESDEV;";

        public abstract string ConnectionString { get; }
        public abstract string ProviderName { get; }

        /// <summary>
        /// Test table name is UT_TABLE
        /// </summary>
        public abstract void CreateTestTables();
        public abstract void DropTestTables();

        public abstract string ParameterPrefix { get; }

        public DbFixture()
        {
            CreateTestTables();
        }

        protected override void Cleanup()
        {
            DropTestTables();
        }
    }
}
