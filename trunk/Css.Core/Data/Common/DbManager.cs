using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Data.Common
{
    public abstract class DbManager
    {
        public static DbManager Manager { get; private set; }
        public static void SetManager(DbManager manager)
        {
            Manager = manager;
        }

        public abstract IDbSetting GetDbSetting(string name);

        public abstract IDbAccesser CreateDbAccesser(IDbSetting setting);

        public abstract IDbTable CreateDbTable(IDbSetting dbSetting, ITableInfo tableInfo);
    }
}
