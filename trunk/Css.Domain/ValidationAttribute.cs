using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ValidationAttribute : Attribute
    {

    }
    /// <summary>
    /// 非空
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RequiredAttribute : ValidationAttribute
    {
    }
    /// <summary>
    /// 非重复
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NotDuplicateAttribute : ValidationAttribute
    {
    }
    /// <summary>
    /// 正则表达式
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RegularAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern"></param>
        public RegularAttribute(string pattern)
        {
            Pattern = pattern;
        }
    }
    /// <summary>
    /// 最大长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MaxLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public int MaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxLength"></param>
        public MaxLengthAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }
    }
    /// <summary>
    /// 最小长度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MinLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public int MinLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minLength"></param>
        public MinLengthAttribute(int minLength)
        {
            MinLength = minLength;
        }
    }
    /// <summary>
    /// 最大值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MaxValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public object MaxValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxValue"></param>
        public MaxValueAttribute(object maxValue)
        {
            MaxValue = maxValue;
        }
    }
    /// <summary>
    /// 最小值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MinValueAttribute : ValidationAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        public object MinValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="minValue"></param>
        public MinValueAttribute(object minValue)
        {
            MinValue = minValue;
        }
    }
}
