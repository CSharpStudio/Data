using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 所有支持的主键的算法容器。
    /// </summary>
    public class KeyProviders
    {
        internal static List<IKeyProvider> Items = new List<IKeyProvider>
        {
            new IntKeyProvider(),
            new StringKeyProvider(),
            new LongKeyProvider(),
            new ObjectKeyProvider(),
            new GuidKeyProvider(),
            new DoubleKeyProvider()
        };

        /// <summary>
        /// 获取指定类型的主键算法。
        /// </summary>
        /// <param name="keyType"></param>
        /// <returns></returns>
        public static IKeyProvider Get(Type keyType)
        {
            //由于量比较少，所以直接避免的性能是最好的。
            foreach (var item in Items)
                if (item.KeyType == keyType)
                    return item;
            throw new NotSupportedException("不支持这个类型的主键：" + keyType);
        }
    }
    internal class DoubleKeyProvider : IKeyProvider
    {
        /// <summary>
        /// 先装箱完成
        /// </summary>
        private object Zero = 0d;

        public Type KeyType
        {
            get { return typeof(double); }
        }

        public object DefaultValue
        {
            get { return Zero; }
        }

        public bool HasId(object id)
        {
            return id != null && id.ConvertTo<double>() > 0;
        }

        public object GenerateId(Type type)
        {
            return RF.Find(type).GetNextId() + DistributedOffset;
        }

        public object GetEmptyIdForRefIdProperty()
        {
            return Zero;
        }

        public object ToNullableValue(object value)
        {
            return HasId(value) ? value : default(double?);
        }

        static double? _distributedOffset;
        /// <summary>
        /// 分布式主键偏移量
        /// </summary>
        static double DistributedOffset
        {
            get
            {
                if (!_distributedOffset.HasValue)
                    _distributedOffset = AppRuntime.Config.Get("db.distributedOffset", 0d);
                return _distributedOffset.Value;
            }
        }
    }
    internal class GuidKeyProvider : IKeyProvider
    {
        private object Empty = Guid.Empty;

        public Type KeyType
        {
            get { return typeof(Guid); }
        }

        public object DefaultValue
        {
            get { return Empty; }
        }

        public bool HasId(object id)
        {
            return id != null && (Guid)id != Guid.Empty;
        }

        public object GenerateId(Type type)
        {
            return Guid.NewGuid();
        }

        public object GetEmptyIdForRefIdProperty()
        {
            return Empty;
        }

        public object ToNullableValue(object value)
        {
            return HasId(value) ? value : default(Guid?);
        }
    }
    internal class IntKeyProvider : IKeyProvider
    {
        /// <summary>
        /// 先装箱完成
        /// </summary>
        private object Zero = 0;

        public Type KeyType
        {
            get { return typeof(int); }
        }

        public object DefaultValue
        {
            get { return Zero; }
        }

        public bool HasId(object id)
        {
            return id != null && Convert.ToInt32(id) > 0;
        }

        public object GenerateId(Type type)
        {
            return RF.Find(type).GetNextId();
        }

        public object GetEmptyIdForRefIdProperty()
        {
            return Zero;
        }

        public object ToNullableValue(object value)
        {
            return HasId(value) ? value : default(int?);
        }
    }
    internal class LongKeyProvider : IKeyProvider
    {
        /// <summary>
        /// 先装箱完成
        /// </summary>
        private object Zero = 0L;

        public Type KeyType
        {
            get { return typeof(long); }
        }

        public object DefaultValue
        {
            get { return Zero; }
        }

        public bool HasId(object id)
        {
            return id != null && Convert.ToInt64(id) > 0;
        }

        public object GenerateId(Type type)
        {
            return RF.Find(type).GetNextId();
        }

        public object GetEmptyIdForRefIdProperty()
        {
            return Zero;
        }

        public object ToNullableValue(object value)
        {
            return HasId(value) ? value : default(long?);
        }
    }
    class ObjectKeyProvider : IKeyProvider
    {
        public Type KeyType
        {
            get { return typeof(object); }
        }

        public object DefaultValue
        {
            get { return null; }
        }

        public object GetEmptyIdForRefIdProperty()
        {
            return null;
        }

        public bool HasId(object id)
        {
            return id != null;
        }

        public object GenerateId(Type type)
        {
            return Guid.NewGuid();
        }

        public object ToNullableValue(object value)
        {
            return value;
        }
    }
    internal class StringKeyProvider : IKeyProvider
    {
        public Type KeyType
        {
            get { return typeof(string); }
        }

        public object DefaultValue
        {
            get { return string.Empty; }
        }

        public bool HasId(object id)
        {
            return id != null && !string.IsNullOrEmpty((string)id);
        }

        public object GenerateId(Type type)
        {
            return Guid.NewGuid().ToString("N");
        }

        public object GetEmptyIdForRefIdProperty()
        {
            return null;
        }

        public object ToNullableValue(object value)
        {
            return value;
        }
    }
}
