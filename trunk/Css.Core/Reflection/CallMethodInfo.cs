using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Reflection
{
    [Serializable]
    public class CallMethodInfo
    {
        /// <summary>
        /// 所有的参数。
        /// </summary>
        public object[] Parameters { get; set; }

        /// <summary>
        /// 数据层查询方法。如果为空，表示使用约定的数据层方法。
        /// </summary>
        public string MethodName { get; set; }
    }
}
