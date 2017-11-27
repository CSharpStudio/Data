using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Data
{
    /// <summary>
    /// 数据库仓库
    /// </summary>
    public interface IDbRepository
    {
        IDbSetting DbSetting { get; }
        IDbAccesser CreateDbAccesser();
        IDbTable DbTable { get; }
    }
}
