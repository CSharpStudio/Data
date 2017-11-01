using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityMetaCreator  :IEntityMetaCreator
    {
        public EntityMeta Create(Type entityType)
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
            return entityMeta;
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
