using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Metadata
{
    public class EntityMetaRepository
    {
        public static EntityMetaRepository Instance = new EntityMetaRepository();

        public static event EventHandler<EntityMetaEventArgs> EntityMetaCreated;

        protected void OnEntityMetaCreated(EntityMeta meta)
        {
            EntityMetaCreated?.Invoke(this, new EntityMetaEventArgs(meta));
        }

        public static event EventHandler<EntityMetaEventArgs> EntityMetaConfigured;

        protected void OnEntityMetaConfigured(EntityMeta meta)
        {
            EntityMetaConfigured?.Invoke(this, new EntityMetaEventArgs(meta));
        }

        Dictionary<Type, EntityMeta> _meta = new Dictionary<Type, EntityMeta>();

        object _syncLock = new object();

        /// <summary>
        /// 查询某个实体类型所对应的实体信息。
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public EntityMeta Find(Type entityType)
        {
            Check.NotNull(entityType, nameof(entityType));
            if (!entityType.IsSubclassOf(typeof(Entity)))
                throw new ArgumentException("类型[{0}]没有继承自Entity,不支持实体元数据");
            EntityMeta meta = null;
            if (!_meta.TryGetValue(entityType, out meta))
            {
                lock (_syncLock)
                {
                    if (!_meta.TryGetValue(entityType, out meta))
                    {
                        meta = FindOrCreate(entityType);
                        _meta.Add(entityType, meta);
                    }
                }
            }
            return meta;
        }

        public void Register(Type entityType, EntityMeta meta)
        {
            _meta.Add(entityType, meta);
        }

        EntityMeta FindOrCreate(Type entityType)
        {
            var entityMeta = new EntityMeta();
            entityMeta.EntityType = entityType;
            var propertyContainer = VarTypeRepository.Instance.GetVarPropertyContainer(entityType);
            foreach (var property in propertyContainer.Properties.OfType<IProperty>())
            {
                if (property is IListProperty)
                    entityMeta.ChildrenProperties.Add(CreateProperty(entityMeta, (IListProperty)property));
                else if (!(property is IRefEntityProperty))
                    entityMeta.Properties.Add(CreateProperty(entityMeta, property));
            }
            OnEntityMetaCreated(entityMeta);
            Config(entityMeta);
            OnEntityMetaConfigured(entityMeta);
            return entityMeta;
        }

        Dictionary<Type, List<IEntityConfig>> _typeConfigurations;
        Dictionary<Type, List<IEntityConfig>> TypeConfigurations
        {
            get
            {
                if (_typeConfigurations == null)
                {
                    _typeConfigurations = new Dictionary<Type, List<IEntityConfig>>();
                    foreach (var plugin in AppRuntime.Modules)
                    {
                        foreach (var type in plugin.Assembly.GetTypes())
                        {
                            if (!type.IsGenericTypeDefinition && typeof(IEntityConfig).IsAssignableFrom(type) && !type.IsAbstract)
                            {
                                var config = Activator.CreateInstance(type) as IEntityConfig;
                                List<IEntityConfig> typeList = null;

                                if (!_typeConfigurations.TryGetValue(config.EntityType, out typeList))
                                {
                                    typeList = new List<IEntityConfig>(2);
                                    _typeConfigurations.Add(config.EntityType, typeList);
                                }

                                typeList.Add(config);
                            }
                        }
                    }
                }
                return _typeConfigurations;
            }
        }

        /// <summary>
        /// 调用配置类进行配置。
        /// </summary>
        /// <param name="em"></param>
        void Config(EntityMeta em)
        {
            var hierachy = em.EntityType.GetHierarchy(typeof(VarObject)).Reverse();
            foreach (var type in hierachy)
            {
                List<IEntityConfig> configList = null;
                if (TypeConfigurations.TryGetValue(type, out configList))
                {
                    foreach (var config in configList)
                    {
                        lock (config)
                        {
                            config.Meta = em;
                            config.ConfigMeta();
                        }
                    }
                }
            }
        }

        protected virtual PropertyMeta CreateProperty(EntityMeta owner, IProperty property)
        {
            if (property is IRefIdProperty)
                return CreateProperty(owner, (IRefIdProperty)property);
            var meta = new PropertyMeta();
            LoadPropertyMeta(owner, meta, property);
            meta.IsReadonly = property.IsReadOnly;
            return meta;
        }

        protected virtual RefPropertyMeta CreateProperty(EntityMeta owner, IRefIdProperty property)
        {
            var meta = new RefPropertyMeta();
            LoadPropertyMeta(owner, meta, property);
            meta.ReferenceType = property.ReferenceType;
            meta.RefProperty = new PropertyInfoMeta();
            LoadPropertyMeta(owner, meta.RefProperty, property.RefEntityProperty);
            meta.Nullable = property.Nullable;
            return meta;
        }

        protected virtual void LoadPropertyMeta(EntityMeta owner, PropertyInfoMeta meta, IProperty property)
        {
            meta.Owner = owner;
            meta.Category = property.Category;
            meta.PropertyName = property.Name;
            meta.PropertyType = property.PropertyType;
            meta.DeclareType = property.DeclareType;
            meta.Serializable = property.Serializable;
        }

        protected virtual ChildPropertyMeta CreateProperty(EntityMeta owner, IListProperty property)
        {
            var meta = new ChildPropertyMeta();
            LoadPropertyMeta(owner, meta, property);
            meta.HasManyType = property.HasManyType;
            return meta;
        }
    }
}
