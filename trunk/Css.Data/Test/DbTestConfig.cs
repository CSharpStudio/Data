using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Test
{
    abstract class DbTestConfig
    {
        public abstract string ConnectionString { get; }
        public abstract string ProviderName { get; }

        /// <summary>
        /// Test table is UT_TABLE(
        ///     [ID]    FLOAT(53)   NOT NULL,
        ///     [NAME]  NVARCHAR(400) NULL,
        ///     [QTY]   FLOAT NULL,
        ///     [FLAG]  BIT NULL,
        ///     [DT]    DATETIME NULL,)
        /// </summary>
        public abstract void CreateTestTables();
        public abstract void DropTestTables();
        /// <summary>
        /// Test stored procedure is UT_PKG.UT_PROCEDURE(
        ///     @param1 int,
        ///     @param2 int,
        ///     @ret_value sql_variant output)
        /// </summary>
        public abstract void CreateTestStoredProcedures();
        public abstract void DropTestStoredProcedures();

        public abstract string ParameterPrefix { get; }
    }
}
