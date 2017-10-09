using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count <= 0;
        }

        /// <summary>
        /// 转换为一个只读的集合。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="orignalCollections"></param>
        /// <returns></returns>
        public static IList<T> AsReadOnly<T>(this IList<T> orignalCollections)
        {
            if (orignalCollections == null) throw new ArgumentNullException("orignalCollections");

            return new System.Collections.ObjectModel.ReadOnlyCollection<T>(orignalCollections);
        }

        /// <summary>
        /// 循环
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (var i in e)
                action(i);
        }

        /// <summary>
        /// 过滤重复
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
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
