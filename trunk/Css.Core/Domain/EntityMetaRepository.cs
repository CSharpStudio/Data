using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// 实体元数据仓库
    /// </summary>
    public class EntityMetaRepository
    {
        /// <summary>
        /// 实体元数据仓库单例
        /// </summary>
        public static EntityMetaRepository Instance = new EntityMetaRepository();

        /// <summary>
        /// 实体元数据创建完成事件
        /// </summary>
        public static event EventHandler<EntityMetaEventArgs> EntityMetaCreated;

        /// <summary>
        /// 实体元数据创建完成
        /// </summary>
        protected void OnEntityMetaCreated(EntityMeta meta)
        {
            EntityMetaCreated?.Invoke(this, new EntityMetaEventArgs(meta));
        }

        /// <summary>
        /// 实体元数据配置完成事件
        /// </summary>
        public static event EventHandler<EntityMetaEventArgs> EntityMetaConfigured;

        /// <summary>
        /// 实体元数据配置完成
        /// </summary>
        /// <param name="meta"></param>
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
            EntityMeta meta = null;
            if (!_meta.TryGetValue(entityType, out meta))
            {
                lock (_syncLock)
                {
                    if (!_meta.TryGetValue(entityType, out meta))
                    {
                        meta = RT.Service.Resolve<IEntityMetaCreator>().Create(entityType);
                        OnEntityMetaCreated(meta);
                        Config(meta);
                        OnEntityMetaConfigured(meta);
                        _meta.Add(entityType, meta);
                    }
                }
            }
            return meta;
        }

        /// <summary>
        /// 注册实体的信息
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="meta"></param>
        public void Register(Type entityType, EntityMeta meta)
        {
            lock (_syncLock)
            {
                _meta.Add(entityType, meta);
            }
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
            var hierachy = em.EntityType.GetHierarchy(typeof(object)).Reverse();
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
    }
}
