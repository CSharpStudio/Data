using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Css.Reflection
{
    public static class PropertyAccessor
    {
        internal delegate V DGet<T, V>(T t);
        internal delegate void DSet<T, V>(T t, V v);
        internal delegate void DRSet<T, V>(ref T t, V v);

        internal static MetaAccessor Create(Type objectType, PropertyInfo pi, MetaAccessor storageAccessor = null)
        {
            Delegate dset = null;
            Delegate drset = null;
            Type dgetType = typeof(DGet<,>).MakeGenericType(objectType, pi.PropertyType);
            MethodInfo getMethod = pi.GetGetMethod(true);

            Delegate dget = Delegate.CreateDelegate(dgetType, getMethod, true);
            if (dget == null)
            {
                throw new Exception("CouldNotCreateAccessorToProperty:{0},{1},{2}".FormatArgs(objectType, pi.PropertyType, pi));
            }

            if (pi.CanWrite)
            {
                if (!objectType.IsValueType)
                {
                    dset = Delegate.CreateDelegate(typeof(DSet<,>).MakeGenericType(objectType, pi.PropertyType), pi.GetSetMethod(true), true);
                }
                else
                {
                    DynamicMethod mset = new DynamicMethod(
                        "xset_" + pi.Name,
                        typeof(void),
                        new Type[] { objectType.MakeByRefType(), pi.PropertyType },
                        true
                        );
                    ILGenerator gen = mset.GetILGenerator();
                    gen.Emit(OpCodes.Ldarg_0);
                    if (!objectType.IsValueType)
                    {
                        gen.Emit(OpCodes.Ldind_Ref);
                    }
                    gen.Emit(OpCodes.Ldarg_1);
                    gen.Emit(OpCodes.Call, pi.GetSetMethod(true));
                    gen.Emit(OpCodes.Ret);
                    drset = mset.CreateDelegate(typeof(DRSet<,>).MakeGenericType(objectType, pi.PropertyType));
                }
            }

            Type saType = (storageAccessor != null) ? storageAccessor.Type : pi.PropertyType;
            return (MetaAccessor)Activator.CreateInstance(
                typeof(Accessor<,,>).MakeGenericType(objectType, pi.PropertyType, saType),
                BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { pi, dget, dset, drset, storageAccessor }, null
                );
        }

        class Accessor<T, V, V2> : MetaAccessor<T, V> where V2 : V
        {
            PropertyInfo pi;
            DGet<T, V> dget;
            DSet<T, V> dset;
            DRSet<T, V> drset;
            MetaAccessor<T, V2> storage;

            internal Accessor(PropertyInfo pi, DGet<T, V> dget, DSet<T, V> dset, DRSet<T, V> drset, MetaAccessor<T, V2> storage)
            {
                this.pi = pi;
                this.dget = dget;
                this.dset = dset;
                this.drset = drset;
                this.storage = storage;
            }
            public override V GetValue(T instance)
            {
                return this.dget(instance);
            }
            public override void SetValue(ref T instance, V value)
            {
                if (this.dset != null)
                {
                    this.dset(instance, value);
                }
                else if (this.drset != null)
                {
                    this.drset(ref instance, value);
                }
                else if (this.storage != null)
                {
                    this.storage.SetValue(ref instance, value.ConvertTo<V2>());
                }
                else
                {
                    throw new Exception("UnableToAssignValueToReadonlyProperty:" + pi);
                }
            }
        }
    }
    public abstract class MetaAccessor<TEntity, TMember> : MetaAccessor
    {
        /// <summary>
        /// The underlying CLR type.
        /// </summary>
        public override Type Type
        {
            get { return typeof(TMember); }
        }
        /// <summary>
        /// Set the boxed value on an instance.
        /// </summary>
        public override void SetBoxedValue(ref object instance, object value)
        {
            TEntity tInst = (TEntity)instance;
            this.SetValue(ref tInst, value.ConvertTo<TMember>());
            instance = tInst;
        }
        /// <summary>
        /// Retrieve the boxed value.
        /// </summary>
        public override object GetBoxedValue(object instance)
        {
            return this.GetValue((TEntity)instance);
        }
        /// <summary>
        /// Gets the strongly-typed value.
        /// </summary>
        public abstract TMember GetValue(TEntity instance);
        /// <summary>
        /// Sets the strongly-typed value
        /// </summary>
        public abstract void SetValue(ref TEntity instance, TMember value);
    }

    public abstract class MetaAccessor
    {
        /// <summary>
        /// The type of the member accessed by this accessor.
        /// </summary>
        public abstract Type Type { get; }
        /// <summary>
        /// Gets the value as an object.
        /// </summary>
        /// <param name="instance">The instance to get the value from.</param>
        /// <returns>Value.</returns>
        public abstract object GetBoxedValue(object instance);
        /// <summary>
        /// Sets the value as an object.
        /// </summary>
        /// <param name="instance">The instance to set the value into.</param>
        /// <param name="value">The value to set.</param>
        public abstract void SetBoxedValue(ref object instance, object value);
        /// <summary>
        /// True if the instance has a loaded or assigned value.
        /// </summary>
        public virtual bool HasValue(object instance)
        {
            return true;
        }
        /// <summary>
        /// True if the instance has an assigned value.
        /// </summary>
        public virtual bool HasAssignedValue(object instance)
        {
            return true;
        }
        /// <summary>
        /// True if the instance has a value loaded from a deferred source.
        /// </summary>
        public virtual bool HasLoadedValue(object instance)
        {
            return false;
        }
    }
}