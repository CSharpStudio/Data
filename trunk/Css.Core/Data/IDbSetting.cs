using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Css.Data
{
    public interface IDbSetting
    {
        string Database { get; }
        string ProviderName { get; }
        string ConnectionString { get; }

        IDbConnection CreateConnection();
    }
}
