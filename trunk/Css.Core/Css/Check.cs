using Css.Properties;
using System;
using System.Collections.Generic;

namespace Css
{
    /// <summary>
    /// 参数检查方法帮助类.
    /// </summary>
    public class Check
    {
        /// <summary>
        /// 断言参数不是null
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">参数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static T NotNull<T>(T value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// 断言字符串参数不是null且不是<see cref="string.Empty"/>
        /// </summary>
        /// <param name="value">字符串参数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static string NotNullOrEmpty(string value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException(Resources.ParameterCannotBeNullOrEmpty.FormatArgs(parameterName));
            }

            return value;
        }

        /// <summary>
        /// 断言字符串参数不是null且不是全空格
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <returns></returns>
        public static string NotNullOrWhiteSpace(string value, string parameterName)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentException(Resources.ParameterCannotBeNullOrWhiteSpace.FormatArgs(parameterName));
            }

            return value;
        }

        /// <summary>
        /// 断言参数不是null且<see cref="ICollection{T}.Count"/>大于0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The collection value</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <returns></returns>
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException(Resources.ParameterCannotBeNullOrEmpty.FormatArgs(parameterName));
            }

            return value;
        }
    }
}

