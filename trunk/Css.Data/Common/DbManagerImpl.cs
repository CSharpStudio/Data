using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Data.Common
{
    public class DbManagerImpl : DbManager
    {
        public override IDbAccesser CreateDbAccesser(IDbSetting setting)
        {
            return DbAccesserFactory.Create(setting);
        }

        public override IDbTable CreateDbTable(IDbSetting dbSetting, ITableInfo tableInfo)
        {
            return DbProvider.CreateTable(dbSetting, tableInfo);
        }

        public override IDbSetting GetDbSetting(string name)
        {
            return DbSetting.FindOrCreate(name);
        }
    }
}
