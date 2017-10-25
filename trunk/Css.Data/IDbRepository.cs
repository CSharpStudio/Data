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
        DbSetting DbSetting { get; }
    }
}
