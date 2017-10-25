using Css.Data;
using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Tests.Data
{
    public class OracleFixture : DbFixture
    {
        public override string ConnectionString
        {
            get { return OracleConnection; }
        }

        public override string ParameterPrefix
        {
            get { return ":"; }
        }

        public override string ProviderName
        {
            get { return DbProvider.Oracle; }
        }

        public override void CreateTestTables()
        {
            using (var dba = new DbAccesser(ConnectionString, ProviderName))
            {
                dba.ExecuteNonQuery(@"create table UT_TABLE
(
  ID    NUMBER not null,
  NAME  VARCHAR2(80),
  FLAG  CHAR(1) null,
  QTY   NUMBER null,
  DT      DATE null
)");
                for (int i = 1; i <= 10; i++)
                    AddTestData(dba, i);

                //包
                dba.ExecuteNonQuery(@"CREATE OR REPLACE PACKAGE UT_PKG IS
type ref_cursor is ref cursor;
  PROCEDURE UT_PROCEDURE(param1 int,param2 int, ret_value in out ref_cursor);
END UT_PKG;");
                //存储过程
                dba.ExecuteNonQuery(@"create or replace package body UT_PKG is
  PROCEDURE UT_PROCEDURE(param1 int,param2 int, ret_value in out ref_cursor) as
  begin
  open ret_value for
  select param1,param2 from dual;
  end;
end UT_PKG;");
            }
        }

        public override void DropTestTables()
        {
            using (var dba = new DbAccesser(ConnectionString, ProviderName))
            {
                dba.ExecuteNonQuery("DROP TABLE UT_TABLE");
                dba.ExecuteNonQuery("DROP PACKAGE UT_PKG");
            }
        }

        protected void AddTestData(DbAccesser dba, int id)
        {
            dba.ExecuteNonQuery("INSERT INTO UT_TABLE(ID,NAME,FLAG) VALUES({0},'TestName {0}','1')".FormatArgs(id));
        }
    }
}
