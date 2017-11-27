using Css.DataPortal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Runtime
{
    public class RuntimeEnvironment
    {
        public static RuntimeEnvironment Instance;

        static RuntimeEnvironment()
        {
            Instance = new RuntimeEnvironment();
            Instance.RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Instance.DllRootDirectory = Instance.RootDirectory;
        }

        /// <summary>
        /// 整个应用程序的根目录
        /// </summary>
        public string RootDirectory { get; set; }

        /// <summary>
        /// Dll 存在的目录路径
        /// （Web 项目的路径是 RootDirectory+"/Bin"）
        /// </summary>
        public string DllRootDirectory { get; set; }

        public bool IsDebuggingEnabled { get; set; }

        public bool IsOnClient()
        {
            return true;
        }

        public bool IsOnServer()
        {
            return true;
        }

        public DataPortalMode DataPortalMode
        {
            get { return DataPortalMode.ConnectDirectly; }
        }
    }
}
