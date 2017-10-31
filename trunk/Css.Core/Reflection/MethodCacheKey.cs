using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Reflection
{
    internal class MethodCacheKey
    {
        internal Type Type;
        internal string MethodName;
        internal Type[] ParamTypes;
        int _hashKey;

        public MethodCacheKey(MethodInfo methodInfo)
        {
            Type = methodInfo.DeclaringType;
            MethodName = methodInfo.Name;
            var parameters = methodInfo.GetParameters();
            ParamTypes = new Type[parameters.Length];

            _hashKey = Type.GetHashCode();
            _hashKey = _hashKey ^ MethodName.GetHashCode();
            for (int i = 0, c = parameters.Length; i < c; i++)
            {
                var item = parameters[i].ParameterType;
                _hashKey = _hashKey ^ item.GetHashCode();
                ParamTypes[i] = item;
            }
        }

        public MethodCacheKey(Type type, string methodName, Type[] paramTypes)
        {
            Type = type;
            MethodName = methodName;
            ParamTypes = paramTypes;

            _hashKey = type.GetHashCode();
            _hashKey = _hashKey ^ methodName.GetHashCode();
            for (int i = 0, c = paramTypes.Length; i < c; i++)
            {
                var item = paramTypes[i];
                _hashKey = _hashKey ^ item.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            var key = obj as MethodCacheKey;
            return key != null &&
                key.Type == Type &&
                key.MethodName == MethodName &&
                ArrayEquals(key.ParamTypes, ParamTypes);
        }

        bool ArrayEquals(Type[] a1, Type[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int pos = 0; pos < a1.Length; pos++)
                if (a1[pos] != a2[pos])
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            return _hashKey;
        }
    }
}