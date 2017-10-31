using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 字符串扩展方法
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 比较两个字符串是否相等。忽略大小写
        /// </summary>
        /// <param name="str">扩展的字符串对象</param>
        /// <param name="target">目标字符串</param>
        /// <returns></returns>
        public static bool CIEquals(this string str, string target)
        {
            return string.Equals(str, target, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 判断字符串是null或者<see cref="string.Empty"/>或者全空格.
        /// </summary>
        /// <param name="str">扩展的字符串对象</param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 判断字符串是null或者<see cref="string.Empty"/>
        /// </summary>
        /// <param name="str">扩展的字符串对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断字符串是否不为Null且不为<see cref="string.Empty"/>
        /// </summary>
        /// <param name="str">扩展的字符串对象</param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 移除字符串前后空格.如果字符串是null,返回<see cref="string.Empty"/>
        /// </summary>
        /// <param name="s">扩展的字符串对象</param>
        /// <returns></returns>
        public static string TrimNull(this string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s.Trim();
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="str">扩展的字符串对象</param>
        /// <param name="args">格式化参数</param>
        /// <returns></returns>
        public static string FormatArgs(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// 使用指定的分隔符把字符串拼接.
        /// </summary>
        /// <param name="arr">扩展的字符串集合</param>
        /// <param name="separator">分隔符</param>
        /// <returns>string</returns>
        public static string Concat(this IEnumerable<string> arr, string separator)
        {
            if (object.Equals(arr, null) || !arr.Any())
                return string.Empty;
            var str = "";
            foreach (var s in arr)
                str += s + separator;
            return str.Remove(str.Length - separator.Length);
        }

        public static string L10N(this string str)
        {
            return Css.RT.ResourceService.GetText(str);
        }
    }
}
