using Css.Data;
using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Tests.Data
{
    public class SqlServerFixture : DbFixture
    {
        public override string ConnectionString
        {
            get { return SqlServerConnection; }
        }

        public override string ParameterPrefix
        {
            get { return "@"; }
        }

        public override string ProviderName
        {
            get { return DbProvider.SqlClient; }
        }

        public override void CreateTestTables()
        {
            using (var dba = new DbAccesser(ConnectionString, ProviderName))
            {
                dba.ExecuteNonQuery(@"CREATE TABLE[dbo].[UT_TABLE](
    [ID]    FLOAT(53)   NOT NULL,
    [NAME]  NVARCHAR(400) NULL,
    [QTY]   FLOAT NULL,
    [FLAG]  BIT   NULL,
    [DT]    DATETIME NULL,
);");

                for (int i = 1; i <= 10; i++)
                    AddTestData(dba, i);

                //测试存储过程
                dba.ExecuteNonQuery(@"CREATE PROCEDURE [dbo].[UT_PKG.UT_PROCEDURE]
	@param1 int = 0,
	@param2 int,
    @ret_value sql_variant OUTPUT
AS
	SELECT @param1, @param2
RETURN 0");
            }
        }

        public override void DropTestTables()
        {
            using (var dba = new DbAccesser(ConnectionString, ProviderName))
            {
                dba.ExecuteNonQuery("DROP TABLE [dbo].[UT_TABLE];");
                dba.ExecuteNonQuery("DROP PROCEDURE [dbo].[UT_PKG.UT_PROCEDURE];");
            }
        }

        protected void AddTestData(DbAccesser dba, int id)
        {
            dba.ExecuteNonQuery("INSERT INTO UT_TABLE(ID,NAME,FLAG) VALUES({0},'TestName {0}',1)".FormatArgs(id));
        }
    }
}
