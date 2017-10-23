using System;
using System.Collections.Generic;
using System.Text;

namespace Css.DataProtal
{
    /// <summary>
    /// 数据门户模式。
    /// </summary>
    public enum DataPortalMode
    {
        /// <summary>
        /// 应用程序直接连接数据。
        /// </summary>
        ConnectDirectly = 0,
        /// <summary>
        /// 应用程序通过服务来连接数据。
        /// </summary>
        ThroughService = 1
    }
}
