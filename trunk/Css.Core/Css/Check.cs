using System;
using System.Collections.Generic;

namespace Css
{
    /// <summary>
    /// A helper class for check arguments.
    /// </summary>
    public class Check
    {
        /// <summary>
        /// Assert value is not null. <see cref="ArgumentNullException"/> will be throw when value is null
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="value">The parameter value</param>
        /// <param name="parameterName">The name of the parameter</param>
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
        /// Assert the string value is not null or <see cref="string.Empty"/>
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static string NotNullOrEmpty(string value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// Assert the string value is not null or white space
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <returns></returns>
        public static string NotNullOrWhiteSpace(string value, string parameterName)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// Assert the collection value is not null and <see cref="ICollection{T}.Count"/> is greater then zero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The collection value</param>
        /// <param name="parameterName">The name of parameter</param>
        /// <returns></returns>
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value, string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            return value;
        }
    }
}

