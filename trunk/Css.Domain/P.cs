using Css.ComponentModel;
using Css.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Css.Domain
{
    public class P<T> where T : VarObject
    {
        public static Property<P> Register<P>(Expression<Func<T, P>> propertyExp, bool serializable = true)
        {
            Check.NotNull(propertyExp, nameof(propertyExp));
            var p = new Property<P>(typeof(T), Reflect<T>.GetProperty(propertyExp).Name, serializable);
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        public static Property<P> Register<P>(string propertyName, bool serializable = true)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            var p = new Property<P>(typeof(T), propertyName, serializable);
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        public static Property<P> RegisterReadOnly<P>(Expression<Func<T, P>> propertyExp, Func<T, P> readOnlyValueProvider, params VarProperty[] dependencies)
        {
            Check.NotNull(propertyExp, nameof(propertyExp));
            var p = new Property<P>(typeof(T), Reflect<T>.GetProperty(propertyExp).Name, false);
            p.AsReadOnly(mpo => readOnlyValueProvider(mpo as T), dependencies);
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        public static Property<P> RegisterReadOnly<P>(string propertyName, Func<T, P> readOnlyValueProvider, params VarProperty[] dependencies)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            var p = new Property<P>(typeof(T), propertyName, false);
            p.AsReadOnly(mpo => readOnlyValueProvider(mpo as T), dependencies);
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        public static Property<P> RegisterExtension<P>(string propertyName, Type declareType, bool serializable = true)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            Check.NotNull(declareType, nameof(declareType));
            var p = new Property<P>(typeof(T), declareType, propertyName, serializable);
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        public static Property<P> RegisterExtensionReadOnly<P>(string propertyName, Type declareType, Func<T, P> readOnlyValueProvider, params VarProperty[] dependencies)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            Check.NotNull(declareType, nameof(declareType));
            Check.NotNull(readOnlyValueProvider, nameof(readOnlyValueProvider));
            var p = new Property<P>(typeof(T), declareType, propertyName, false);
            p.AsReadOnly(mpo => readOnlyValueProvider(mpo as T), dependencies);
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        //#region RegisterExtensionReference

        //public static IRefIdProperty RegisterExtensionRefId<TKey>(string propertyName, Type declareType, ReferenceType referenceType, bool isKeyNullable = true)
        //    where TKey : struct
        //{
        //    return RegisterExtensionRefId(propertyName, declareType, new RegisterRefIdArgs<TKey> { ReferenceType = referenceType }, isKeyNullable);
        //}

        //public static IRefIdProperty RegisterExtensionRefId<TKey>(string propertyName, Type declareType, RegisterRefIdArgs<TKey> args, bool isKeyNullable = true)
        //    where TKey : struct
        //{
        //    var property = new RefIdProperty<TKey>(typeof(TEntity), declareType, propertyName, args)
        //    {
        //        ReferenceType = args.ReferenceType,
        //        Nullable = isKeyNullable
        //    };

        //    ManagedPropertyRepository.Instance.RegisterProperty(property);

        //    return property;
        //}
        //public static RefEntityProperty<TRefEntity> RegisterExtensionRef<TRefEntity>(string propertyName, Type declareType, IRefIdProperty refIdProperty)
        //   where TRefEntity : Entity
        //{
        //    if (refIdProperty == null) throw new ArgumentNullException("refIdProperty", "必须指定引用 Id 属性，将为其建立关联。");

        //    var defaultMeta = new PropertyMetadata<Entity>();

        //    var property = new RefEntityProperty<TRefEntity>(typeof(TEntity), declareType, propertyName, defaultMeta)
        //    {
        //        RefIdProperty = refIdProperty
        //    };

        //    //默认只从服务端序列化到客户端。
        //    defaultMeta.Serializable = PlatformEnvironment.IsOnServer();

        //    ManagedPropertyRepository.Instance.RegisterProperty(property);

        //    return property;
        //}

        //public static RefEntityProperty<TRefEntity> RegisterExtensionRef<TRefEntity>(string propertyName, Type declareType, RegisterRefArgs args)
        //    where TRefEntity : Entity
        //{
        //    if (args == null) throw new ArgumentNullException("args");
        //    if (args.RefIdProperty == null) throw new ArgumentNullException("args.RefIdProperty", "必须指定引用 Id 属性，将为其建立关联。");

        //    //简单地，直接把 Args 作为 defaultMeta
        //    var defaultMeta = args;

        //    var property = new RefEntityProperty<TRefEntity>(typeof(TEntity), declareType, propertyName, defaultMeta)
        //    {
        //        RefIdProperty = args.RefIdProperty,
        //        Loader = args.Loader
        //    };

        //    //默认只从服务端序列化到客户端。
        //    (defaultMeta as PropertyMetadata<Entity>).Serializable = args.Serializable.GetValueOrDefault(PlatformEnvironment.IsOnServer());

        //    ManagedPropertyRepository.Instance.RegisterProperty(property);

        //    return property;
        //}

        //#endregion

        /// <summary>
        /// 声明一个引用 Id 属性
        /// </summary>
        /// <param name="propertyExp">指向相应 CLR 的表达式。</param>
        /// <param name="referenceType">引用的类型</param>
        /// <returns></returns>
        public static IRefIdProperty RegisterRefId<TKey>(Expression<Func<T, TKey?>> propertyExp, ReferenceType referenceType, bool serializable = true)
            where TKey : struct
        {
            Check.NotNull(propertyExp, nameof(propertyExp));
            var propertyInfo = Reflect<T>.GetProperty(propertyExp);
            var p = new RefIdProperty<TKey>(typeof(T), propertyInfo.Name, serializable)
            {
                ReferenceType = referenceType,
                Nullable = true
            };
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        /// <summary>
        /// 声明一个引用 Id 属性
        /// </summary>
        /// <param name="propertyExp">指向相应 CLR 的表达式。</param>
        /// <param name="referenceType">引用的类型</param>
        /// <returns></returns>
        public static IRefIdProperty RegisterRefId<TKey>(Expression<Func<T, TKey>> propertyExp, ReferenceType referenceType, bool serializable = true)
        {
            Check.NotNull(propertyExp, nameof(propertyExp));
            var propertyInfo = Reflect<T>.GetProperty(propertyExp);
            var p = new RefIdProperty<TKey>(typeof(T), propertyInfo.Name, serializable)
            {
                ReferenceType = referenceType,
                Nullable = propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.IsNullable()
            };
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        public static IRefIdProperty RegisterRefId<TKey>(string propertyName, ReferenceType referenceType, Type propertyType, bool serializable = true)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            Check.NotNull(propertyType, nameof(propertyType));
            var p = new RefIdProperty<TKey>(typeof(T), propertyName, serializable)
            {
                ReferenceType = referenceType,
                Nullable = propertyType.IsClass || propertyType.IsNullable()
            };
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        /// <summary>
        /// 声明一个引用实体属性。
        /// </summary>
        /// <typeparam name="TRefEntity"></typeparam>
        /// <param name="propertyExp">指向引用实体属性的表达式。</param>
        /// <param name="refIdProperty">对应的引用 Id 属性，将为其建立关联。</param>
        /// <returns></returns>
        public static RefEntityProperty<TRefEntity> RegisterRef<TRefEntity>(Expression<Func<T, TRefEntity>> propertyExp, IRefIdProperty refIdProperty, RefEntityLoader loader = null, bool? serializable = null)
            where TRefEntity : Entity
        {
            Check.NotNull(propertyExp, nameof(propertyExp));
            Check.NotNull(refIdProperty, nameof(refIdProperty));

            //默认只从服务端序列化到客户端。
            var p = new RefEntityProperty<TRefEntity>(typeof(T), Reflect<T>.GetProperty(propertyExp).Name, serializable.GetValueOrDefault(AppRuntime.Environment.IsOnServer()))
            {
                RefIdProperty = refIdProperty,
                Loader = loader
            };
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        public static RefEntityProperty<TRefEntity> RegisterRef<TRefEntity>(string propertyName, IRefIdProperty refIdProperty, RefEntityLoader loader = null, bool? serializable = null)
            where TRefEntity : Entity
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            Check.NotNull(refIdProperty, nameof(refIdProperty));

            //默认只从服务端序列化到客户端。
            var p = new RefEntityProperty<TRefEntity>(typeof(T), propertyName, serializable.GetValueOrDefault(AppRuntime.Environment.IsOnServer()))
            {
                RefIdProperty = refIdProperty,
                Loader = loader
            };
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        /// <summary>
        /// 注册一个列表属性
        /// </summary>
        /// <typeparam name="TEntityList">The type of the entity list.</typeparam>
        /// <param name="propertyExp">The property exp.</param>
        /// <returns></returns>
        public static ListProperty<TEntityList> RegisterList<TEntityList>(Expression<Func<T, TEntityList>> propertyExp, HasManyType hasManyType = HasManyType.Composition, ListLoaderProvider loader = null, bool serializable = true)
            where TEntityList : IEntityList
        {
            Check.NotNull(propertyExp, nameof(propertyExp));
            var p = new ListProperty<TEntityList>(typeof(T), Reflect<T>.GetProperty(propertyExp).Name, serializable)
            {
                HasManyType = hasManyType,
                DataProvider = loader
            };
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        static ListProperty<TEntityList> RegisterList<TEntityList>(string propertyName, HasManyType hasManyType = HasManyType.Composition, ListLoaderProvider loader = null, bool serializable = true)
            where TEntityList : IEntityList
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            var p = new ListProperty<TEntityList>(typeof(T), propertyName, serializable)
            {
                HasManyType = hasManyType,
                DataProvider = loader
            };
            VarPropertyRepository.RegisterProperty(p);
            return p;
        }

        //#endregion

        //#region RegisterExtensionList

        //public static LisP<TEntityList> RegisterExtensionList<TEntityList>(string propertyName, Type declareType)
        //    where TEntityList : EntityList
        //{
        //    var args = new LisPMeta();
        //    var meta = new LisPMetadata<TEntityList>(args.DataProvider);

        //    var property = new LisP<TEntityList>(typeof(TEntity), declareType, propertyName, meta);

        //    property._hasManyType = args.HasManyType;

        //    ManagedPropertyRepository.Instance.RegisterProperty(property);

        //    return property;
        //}

        //public static LisP<TEntityList> RegisterExtensionList<TEntityList>(string propertyName, Type declareType, LisPMeta args)
        //    where TEntityList : EntityList
        //{
        //    var meta = new LisPMetadata<TEntityList>(args.DataProvider);

        //    var property = new LisP<TEntityList>(typeof(TEntity), declareType, propertyName, meta);

        //    property._hasManyType = args.HasManyType;

        //    ManagedPropertyRepository.Instance.RegisterProperty(property);

        //    return property;
        //}

    }
}
