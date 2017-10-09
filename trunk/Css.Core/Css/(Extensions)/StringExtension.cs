using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class StringExtension
    {
        /// <summary>
        /// 比较两个字符串是否相等。忽略大小写
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CIEquals(this string str, string target)
        {
            return string.Equals(str, target, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// judge this string is :
        /// null/String.Empty/all white spaces.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 判断字符串是否不为Null且不为String.Empty 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrLengthBetween(this string str, int minLength, int maxLength)
        {
            if (str == null)
            {
                return true;
            }
            return str.IsLengthBetween(minLength, maxLength);
        }

        public static bool IsLengthBetween(this string str, int minLength, int maxLength)
        {
            if (str == null)
            {
                return false;
            }
            int length = str.Length;
            return length <= maxLength && length >= minLength;
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the current System.String object.
        /// if it is null, return the string.Empty.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string TrimNull(this string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s.Trim();
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatArgs(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

        /// <summary>
        /// Concat strings by specified separator.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
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

        //public static string L10N(this string str)
        //{
        //    return Css.RT.ResourceService.GetText(str);
        //}
    }
}
