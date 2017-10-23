using Css;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// 集合的扩展方法
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// 检查集合不是null且<see cref="ICollection{T}.Count"/>大于0
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// 转换为一个只读的集合。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orignalCollections">原始集合</param>
        /// <returns></returns>
        public static IList<T> AsReadOnly<T>(this IList<T> orignalCollections)
        {
            Check.NotNull(orignalCollections, nameof(orignalCollections));

            return new System.Collections.ObjectModel.ReadOnlyCollection<T>(orignalCollections);
        }

        /// <summary>
        /// 循环执行<see cref="Action{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="action">执行的操作</param>
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (var i in e)
                action(i);
        }

        /// <summary>
        /// 过滤集合的重复项目
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">集合</param>
        /// <param name="comparer">比较器</param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> comparer)
        {
            return source.Distinct(new DistinctComparer<T>(comparer));
        }
    }

    class DistinctComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _comparer;

        public DistinctComparer(Func<T, T, bool> comparer)
        {
            _comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return _comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
