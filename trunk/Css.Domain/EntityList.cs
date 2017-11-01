using Css.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityList<T> : ObservableList<T>, IEntityList where T : Entity
    {
        public EntityList()
        {
            EntityType = typeof(T);
        }

        public Type EntityType { get; set; }

        Type IEntityList.EntityType
        {
            get { return EntityType; }
        }

        IList IEntityList.Deleted
        {
            get { return _deleted; }
        }

        /// <summary>
        /// Gets a value indicating whether this object's data has been changed.
        /// </summary>
        [Browsable(false)]
        public bool IsDirty
        {
            get
            {
                if (_deleted.Any())
                    return true;

                // run through all the child objects
                // and if any are dirty then then
                // collection is dirty
                foreach (T child in this)
                    if (child.DataState == DataState.Modified)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// 实体对应的仓库。
        /// </summary>
        [NonSerialized]
        IRepository _repository;

        public int Total { get; set; }

        /// <summary>
        /// 获取该实体列表对应的仓库类。
        /// </summary>
        /// <returns></returns>
        public IRepository GetRepository()
        {
            return _repository ?? (_repository = RepositoryFactory.Find(EntityType));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        void IEntityList.SyncParentEntityId(IEntity parent)
        {
            var property = GetRepository().GetParentProperty();
            this.ForEach(child =>
            {
                //注意，由于实体可能并没有发生改变，而只是 Id 变了，
                //所以在设置的时候，先设置 Id，然后设置 Entity。
                child.SetRefId(parent.GetId(), property.RefIdProperty);
                child.SetRefEntity(parent, property);
            });
        }

        public void SetRefParent(IEntity entity)
        {
            var property = GetRepository().GetParentProperty();
            foreach (T item in Items)
            {
                item.SetRefEntity(entity, property);
            }
        }
    }
}
