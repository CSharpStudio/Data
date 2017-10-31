using Css.Data;
using Css.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    /// <summary>
    /// Entity serivce is a remote service and provide basic methods for entity.
    /// </summary>
    public class EntityService : RemoteService
    {
        public virtual void Save(IDomain domain)
        {
            Check.NotNull(domain, nameof(domain));
            domain.GetRepository().Save(domain);
        }

        public virtual Entity GetById(Type entityType, object id)
        {
            return RF.Find(entityType).GetById(id);
        }

        public virtual object GetFirst(Type entityType, string criteria)
        {
            Func<string, Entity> f = GetFirst<Entity>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(entityType).Invoke(this, new[] { criteria });
        }

        public virtual IEntityList GetList(Type entityType, string criteria)
        {
            Func<string, EntityList<Entity>> f = GetList<Entity>;
            return f.Method.GetGenericMethodDefinition().MakeGenericMethod(entityType).Invoke(this, new[] { criteria }) as IEntityList;
        }

        /// <summary>
        /// 更新实体部分字段或者批量更新
        /// <para>
        /// RF.Update&lt;User&gt;().Set(p=>p.Name, "NewName").Where(p=>p.Id == 1);
        /// </para>
        /// </summary>
        protected IEntityUpdate<T> Update<T>() where T : Entity
        {
            var repo = RF.Find<T>();
            return new EntityUpdate<T>(repo);
        }
        /// <summary>
        /// 批量删除
        /// <para>
        /// RF.Delete&lt;User&gt;().Where(p=>p.Name.Contains("admin"));
        /// </para>
        /// </summary>
        protected IEntityDelete<T> Delete<T>() where T : Entity
        {
            var repo = RF.Find<T>();
            return new EntityDelete<T>(repo);
        }

        [Local]
        protected IEntityQueryer<T> Query<T>(string alias = null)
        {
            var repo = RF.Find<T>();
            return new EntityQueryer<T>(repo, alias);
        }

        [Local]
        protected internal T GetFirst<T>(string criteria) where T : Entity
        {
            return Query<T>().Where(criteria).FirstOrDefault();
        }

        [Local]
        protected internal EntityList<T> GetList<T>(string criteria) where T : Entity
        {
            return Query<T>().Where(criteria).ToList();
        }
    }

    public class ES
    {
        public static void Save(IDomain domain)
        {
            RT.Service.Resolve<EntityService>().Save(domain);
        }

        public static T GetById<T>(object id) where T : Entity
        {
            return RT.Service.Resolve<EntityService>().GetById(typeof(T), id) as T;
        }

        public static T GetFrist<T>(string criteria) where T : Entity
        {
            return RT.Service.Resolve<EntityService>().GetFirst(typeof(T), criteria) as T;
        }

        public static EntityList<T> GetList<T>(string criteria) where T : Entity
        {
            return RT.Service.Resolve<EntityService>().GetList(typeof(T), criteria) as EntityList<T>;
        }
    }
}
