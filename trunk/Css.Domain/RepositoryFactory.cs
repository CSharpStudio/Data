using Css.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Css.Domain
{
    public abstract class RepositoryFactory
    {
        /// <summary>
        /// 某个实体提交前事件。
        /// 静态事件，不能经常修改此列表。建议在插件初始化时使用。
        /// </summary>
        public static event EventHandler<EntitySubmittingEventArgs> Submitting;

        internal static bool OnSubmitting(SubmitArgs e)
        {
            if (Submitting != null)
            {
                var args = new EntitySubmittingEventArgs(e.Entity, e.Action);
                Submitting(e.Entity, args);
                return !args.Cancel;
            }
            return true;
        }
        /// <summary>
        /// 某个实体提交前事件。
        /// 静态事件，不能经常修改此列表。建议在插件初始化时使用。
        /// </summary>
        public static event EventHandler<EntitySubmittedEventArgs> Submitted;

        internal static void OnSubmitted(SubmitArgs e)
        {
            Submitted?.Invoke(e.Entity, new EntitySubmittedEventArgs(e.Entity, e.Action));
        }

        /// <summary>
        /// 某个实体增加前事件。
        /// 静态事件，不能经常修改此列表。建议在插件初始化时使用。
        /// </summary>
        public static event EventHandler<EntityEventArgs> Inserting;

        internal static bool OnInserting(Entity e)
        {
            if (Inserting != null)
            {
                var args = new EntityEventArgs(e);
                Inserting(e, args);
                return !args.Cancel;
            }
            return true;
        }
        /// <summary>
        /// 某个实体修改前事件。
        /// 静态事件，不能经常修改此列表。建议在插件初始化时使用。
        /// </summary>
        public static event EventHandler<EntityEventArgs> Updating;

        internal static bool OnUpdating(Entity e)
        {
            if (Updating != null)
            {
                var args = new EntityEventArgs(e);
                Updating(e, args);
                return !args.Cancel;
            }
            return true;
        }

        /// <summary>
        /// 某个实体删除前事件。
        /// 静态事件，不能经常修改此列表。建议在插件初始化时使用。
        /// </summary>
        public static event EventHandler<EntityEventArgs> Deleting;

        internal static bool OnDeleting(Entity e)
        {
            if (Deleting != null)
            {
                var args = new EntityEventArgs(e);
                Deleting(e, args);
                return !args.Cancel;
            }
            return true;
        }

        /// <summary>
        /// 查询实体的事件。
        /// 静态事件，不能经常修改此列表。建议在插件初始化时使用。
        /// </summary>
        //public static event EventHandler<QueryingEventArgs> Querying;

        //public static event EventHandler<ExecutingEventArgs> Executing;

        internal static RepositoryHost Host { get; } = new RepositoryHost();

        /// <summary>
        /// 用于查找指定实体的仓库。
        /// </summary>
        /// <returns></returns>
        public static EntityRepository Find(Type entityType)
        {
            return Host.Find(entityType);
        }
        /// <summary>
        /// 用于查找指定实体的仓库。
        /// </summary>
        /// <returns></returns>
        public static EntityRepository Find<T>()
        {
            return Host.Find(typeof(T));
        }

        internal class RepositoryHost
        {
            IDictionary<Type, EntityRepository> _repoByEntityType = new SortedDictionary<Type, EntityRepository>(TypeNameComparer.Instance);

            EntityRepository _lastRepository;
            /// <summary>
            /// 用于查找指定实体的仓库。
            /// </summary>
            /// <returns></returns>
            public EntityRepository Find(Type entityType)
            {
                var result = _lastRepository;
                if (result != null && result.EntityType == entityType) return result;

                if (!_repoByEntityType.TryGetValue(entityType, out result))
                {
                    //只有实体才能有对应的仓库类型。
                    if (typeof(Entity).IsAssignableFrom(entityType))
                    {
                        lock (_repoByEntityType)
                        {
                            result = FindOrCreateWithoutLock(entityType);
                        }
                    }
                }

                _lastRepository = result;

                return result;
            }

            EntityRepository FindOrCreateWithoutLock(Type entityType)
            {
                EntityRepository result = null;

                if (!_repoByEntityType.TryGetValue(entityType, out result))
                {
                    result = DoCreate(entityType);
                    _repoByEntityType.Add(entityType, result);
                }

                return result;
            }

            EntityRepository DoCreate(Type entityType)
            {
                try
                {
                    var repoType = ConventionRepositoryForEntity(entityType);
                    if (repoType != null)
                    {
                        if (repoType.IsAbstract) throw new InvalidProgramException(repoType.FullName + " 仓库类型是抽象的，无法创建。");
                        EntityRepository repo = Activator.CreateInstance(repoType, true) as EntityRepository;

                        return repo;
                    }
                    var defaultRepository = typeof(EntityRepository<>).MakeGenericType(entityType);
                    return Activator.CreateInstance(defaultRepository) as EntityRepository;
                }
                catch (Exception exc)
                {
                    throw new Exception(entityType.GetQualifiedName(), exc);
                }
            }

            public Type ConventionRepositoryForEntity(Type entityType)
            {
                var attrEntity = entityType.GetCustomAttribute<EntityAttribute>();
                if (attrEntity != null && attrEntity.RepositoryType != null)
                {
                    if (attrEntity.RepositoryType.IsGenericTypeDefinition)
                        return attrEntity.RepositoryType.MakeGenericType(entityType);
                    return attrEntity.RepositoryType;
                }

                string rpName = entityType.FullName + "Repository";
                Type rpType = entityType.Assembly.GetType(rpName);
                if (rpType != null)
                    return rpType;


                return typeof(EntityRepository<>).MakeGenericType(entityType);
            }

            public Type ConventionListForEntity(Type entityType)
            {
                var attrEntity = entityType.GetCustomAttribute<EntityAttribute>();
                if (attrEntity != null && attrEntity.ListType != null)
                {
                    if (attrEntity.ListType.IsGenericTypeDefinition)
                        return attrEntity.ListType.MakeGenericType(entityType);
                    return attrEntity.ListType;
                }

                string listTypeName = entityType.FullName + "List";
                Type listType = entityType.Assembly.GetType(listTypeName);
                if (listType != null)
                    return listType;

                return typeof(EntityList<>).MakeGenericType(entityType);
            }
        }
    }

    public abstract class RF : RepositoryFactory
    {

    }
}
