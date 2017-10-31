using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Text;

namespace Css.DataProtal
{
    [DataContract, Serializable]
    public class ApiResponse
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public object Result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public HybridDictionary Context { get; set; } = new HybridDictionary();
    }
}
