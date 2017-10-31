using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Text;

namespace Css.DataProtal
{
    [DataContract, Serializable]
    public class ApiRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ApiType { get; set; }

        /// <summary>
        /// 所有的参数。
        /// </summary>
        [DataMember]
        public ApiMethodParameter[] Parameters { get; set; }

        /// <summary>
        /// 方法名称。
        /// </summary>
        [DataMember]
        public string Method { get; set; }

        /// <summary>
        /// Data portal context from client.
        /// </summary>
        [DataMember]
        public HybridDictionary Context { get; set; } = new HybridDictionary();
    }

    [DataContract, Serializable]
    public class ApiMethodParameter
    {
        /// <summary>
        /// 参数值。
        /// </summary>
        [DataMember]
        public object Value { get; set; }
    }
}
