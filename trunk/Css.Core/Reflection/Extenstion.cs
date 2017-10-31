using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Reflection
{
    public static class Extenstion
    {
        static object typeSyncLock = new object();
        static object metaSyncLock = new object();

        public static MetaAccessor GetMetaAccessor(this Type type, PropertyInfo pi)
        {
            Dictionary<PropertyInfo, MetaAccessor> map;
            if (!Meta.TryGetValue(type, out map))
            {
                lock (typeSyncLock)
                {
                    if (!Meta.TryGetValue(type, out map))
                    {
                        map = new Dictionary<PropertyInfo, MetaAccessor>();
                        Meta[type] = map;
                    }
                }
            }
            MetaAccessor result;
            if (!map.TryGetValue(pi, out result))
            {
                lock (metaSyncLock)
                {
                    if (!map.TryGetValue(pi, out result))
                    {

                        result = PropertyAccessor.Create(type, pi);
                        map[pi] = result;
                    }
                }
            }
            return result;
        }

        static Dictionary<Type, Dictionary<PropertyInfo, MetaAccessor>> Meta = new Dictionary<Type, Dictionary<PropertyInfo, MetaAccessor>>();

        public static object ToObject(this IDataReader reader, NewExpression expr)
        {
            object[] values = new object[expr.Arguments.Count];
            for (int i = 0; i < expr.Arguments.Count; i++)
            {
                var member = expr.Members[i] as PropertyInfo;
                var value = reader.GetValue(i);
                if (value != DBNull.Value)
                    values[i] = value.ConvertTo(member.PropertyType);
                else
                    values[i] = Activator.CreateInstance(member.PropertyType);
            }
            return expr.Constructor.Invoke(values);
        }

        public static dynamic ToDynamic(this IDataReader reader)
        {
            dynamic d = new ExpandoObject();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    ((IDictionary<string, object>)d).Add(reader.GetName(i), reader.GetValue(i));
                }
                catch
                {
                    ((IDictionary<string, object>)d).Add(reader.GetName(i), null);
                }
            }
            return d;
        }
    }
}
