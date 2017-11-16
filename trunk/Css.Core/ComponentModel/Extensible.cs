using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Css.ComponentModel
{
    /// <summary>
    /// 可扩展对象，扩展的属性存于<see cref="Hashtable"/>
    /// </summary>
    [Serializable]
    public class Extensible
    {
        [XmlElement("ValueTable")]
        Hashtable valueTable = new Hashtable();

        /// <summary>
        /// 获取扩展属性值
        /// </summary>
        /// <typeparam name="T">属性类型参数</typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <param name="defaultValue">属性默认值</param>
        /// <returns></returns>
        public T Get<T>(string propertyName, T defaultValue = default(T))
        {
            Check.NotNull(propertyName, nameof(propertyName));
            if (valueTable.ContainsKey(propertyName))
                return valueTable[propertyName].ConvertTo<T>();
            return defaultValue;
        }

        /// <summary>
        /// 设置扩展属性的值
        /// </summary>
        /// <typeparam name="T">属性类型参数</typeparam>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value"></param>
        public void Set<T>(string propertyName, T value)
        {
            Check.NotNull(propertyName, nameof(propertyName));
            valueTable[propertyName] = value;
        }
    }
}
