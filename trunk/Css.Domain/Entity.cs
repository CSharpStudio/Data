using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Css.Domain
{
    [Serializable]
    public abstract class Entity : VarObject, IEntity
    {
        #region Status

        [NonSerialized]
        EventHandler _stateChanged;

        public event EventHandler StateChanged
        {
            add { _stateChanged = (EventHandler)Delegate.Combine(_stateChanged, value); }
            remove { _stateChanged = (EventHandler)Delegate.Remove(_stateChanged, value); }
        }

        DataState _state = DataState.Created;
        /// <summary>
        /// 状态
        /// </summary>
        [Browsable(false)]
        public virtual DataState DataState
        {
            get { return _state; }
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged();
                }
            }
        }

        protected virtual void OnStateChanged()
        {
            _stateChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Change the DataState to Created.
        /// </summary>
        public virtual void MarkCreated()
        {
            DataState = DataState.Created;
        }

        /// <summary>
        /// Change the DataState to Modified.
        /// </summary>
        public virtual void MarkModified()
        {
            DataState = DataState.Modified;
        }

        /// <summary>
        /// Change the DataState to Deleted.
        /// </summary>
        public virtual void MarkDeleted()
        {
            DataState = DataState.Deleted;
        }

        /// <summary>
        /// Change the DataState to Normal.
        /// </summary>
        public virtual void ResetState()
        {
            DataState = DataState.Normal;
        }

        protected override void OnValueChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnValueChanged(propertyName, oldValue, newValue);
            if (DataState == DataState.Normal)
                MarkModified();
        }

        #endregion

        #region Id

        public static IProperty IdProperty = P<Entity>.Register<object>("Id");

        /// <summary>
        /// 获取ID
        /// </summary>
        /// <returns></returns>
        public object GetId()
        {
            return base.GetValue(IdProperty);
        }

        /// <summary>
        /// 设置ID
        /// </summary>
        /// <param name="id"></param>
        public void SetId(object id)
        {
            base.SetValue(id, IdProperty);
        }

        public void GenerateId()
        {
            var provider = KeyProviders.Get(IdProperty.PropertyType);
            SetId(provider.GenerateId(GetType()));
        }

        #endregion

        #region LazyLoad

        /// <summary>
        /// 延迟加载子对象的集合。
        /// </summary>
        /// <typeparam name="TCollection">子对象集合类型</typeparam>
        /// <param name="propertyInfo">当前属性的元信息</param>
        /// <returns></returns>
        protected TCollection GetLazyList<TCollection>(IListProperty<TCollection> listProperty)
            where TCollection : class, IEntityList
        {
            return GetLazyList(listProperty as IListProperty) as TCollection;
        }

        protected object GetLazyList(IListProperty listProperty)
        {
            Check.NotNull(listProperty, nameof(listProperty));
            object data = null;

            if (FieldData.FieldExists(listProperty))
            {
                data = base.GetValue(listProperty);
                if (data != null) return data;
            }

            if (DataState == DataState.Created)
            {
                data = Activator.CreateInstance(listProperty.PropertyType);
            }
            else
            {
                var dataProvider = listProperty.DataProvider;
                if (dataProvider != null)
                {
                    data = dataProvider(this);
                }
                else
                {
                    var repo = RF.Find(listProperty.EntityType);
                    data = repo.GetByParentId(GetId());
                    ((IEntityList)data).SetRefParent(this);
                }
            }

            LoadValue(data, listProperty);

            return data;
        }

        /// <summary>
        /// 获取指定引用 id 属性对应的 id 的可空类型返回值。
        /// </summary>
        /// <param name="property"></param>
        /// <returns>本方法为兼容值类型而使用。不论 Id 是值类型、还是引用类型，都可能返回 null。</returns>
        public object GetRefNullableId(IRefIdProperty property)
        {
            Check.NotNull(property, nameof(property));
            var value = base.GetValue(property);
            return property.KeyProvider.ToNullableValue(value);
        }

        /// <summary>
        /// 设置指定引用 id 属性对应的 id 的可空类型值。
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value">本方法为兼容值类型而使用。不论外键是否为值类型，都可以传入 null。</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public void SetRefNullableId(object value, IRefIdProperty property)
        {
            Check.NotNull(property, nameof(property));
            if (value == null) { value = property.KeyProvider.GetEmptyIdForRefIdProperty(); }
            SetRefId(value, property);
        }

        /// <summary>
        /// 获取指定引用 id 属性对应的 id 的返回值。
        /// </summary>
        /// <param name="property"></param>
        /// <returns>如果 Id 是值类型，则这个函数的返回值不会是 null；如果是引用类型，则可能返回 null。</returns>
        public object GetRefId(IRefIdProperty property)
        {
            Check.NotNull(property, nameof(property));
            return base.GetValue(property);
        }

        /// <summary>
        /// 设置指定引用 id 属性对应的 id 的值。
        /// 
        /// 在引用 id 变化时，会同步相应的引用实体属性。
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value">外键如果是值类型，则不能传入 null。</param>
        /// <param name="source"></param>
        /// <returns></returns>
        public void SetRefId(object value, IRefIdProperty property)
        {
            Check.NotNull(property, nameof(property));
            value = value.ConvertTo(property.KeyProvider.KeyType);
            var oldValue = GetRefId(property);
            if (!object.Equals(oldValue, value))
            {
                var entityProperty = property.RefEntityProperty;
                var entity = base.GetValue(entityProperty) as Entity;
                if (entity != null && !object.Equals(entity.GetId(), value)) { FieldData.RemoveField(entityProperty); }
                FieldData.SetField(property, value);
                RasiePropertyValueChanged(property, oldValue, value);
                RasiePropertyValueChanged(entityProperty, entity, null);
            }
        }

        /// <summary>
        /// 以懒加载的方式获取某个引用实体的值。
        /// </summary>
        /// <typeparam name="TRefEntity"></typeparam>
        /// <param name="entityProperty"></param>
        /// <returns></returns>
        public TRefEntity GetRefEntity<TRefEntity>(RefEntityProperty<TRefEntity> entityProperty)
            where TRefEntity : Entity
        {
            return GetRefEntity(entityProperty as IRefEntityProperty) as TRefEntity;
        }

        /// <summary>
        /// 以懒加载的方式获取某个引用实体的值。
        /// </summary>
        /// <param name="entityProperty"></param>
        /// <returns></returns>
        public Entity GetRefEntity(IRefEntityProperty entityProperty)
        {
            Check.NotNull(entityProperty, nameof(entityProperty));
            object value = base.GetValue(entityProperty);
            if (!_settingEntity && value == null)
            {
                var idProperty = entityProperty.RefIdProperty;
                var id = GetRefId(idProperty);
                if (idProperty.KeyProvider.HasId(id))
                {
                    value = entityProperty.Load(id, this);
                    if (value != null)
                    {
                        LoadValue(value, entityProperty);
                    }
                }
            }
            return value as Entity;
        }

        [NonSerialized]
        bool _settingEntity;

        void CheckSetListProperty(IVarProperty property)
        {
            if (property is IListProperty)
            {
                //防止外界使用 SetProperty 方法来操作列表属性。
                throw new InvalidOperationException(string.Format("{0} 是列表属性，不能使用 SetProperty 方法直接设置。请使用 GetLazyList 方法获取，或使用 LoadProperty 方法进行加载。", property));
            }
        }

        /// <summary>
        /// 设置指定引用实体属性的值。
        /// 在实体属性变化时，会同步相应的引用 Id 属性。
        /// </summary>
        /// <param name="entityProperty">The entity property.</param>
        /// <param name="value">The value.</param>
        /// <param name="source">The source.</param>
        public void SetRefEntity(Entity value, IRefEntityProperty entityProperty)
        {
            Check.NotNull(entityProperty, nameof(entityProperty));
            var oldEntity = base.GetValue(entityProperty) as Entity;
            try
            {
                _settingEntity = true;
                var idProperty = entityProperty.RefIdProperty;
                var oldId = base.GetValue(idProperty);
                if (oldEntity != value || (value == null && idProperty.KeyProvider.HasId(oldId)))
                {
                    var newId = value == null ? idProperty.KeyProvider.GetEmptyIdForRefIdProperty() : value.GetId();
                    LoadValue(newId, idProperty);
                    FieldData.SetField(entityProperty, value);
                    RasiePropertyValueChanged(entityProperty, oldEntity, value);
                    RasiePropertyValueChanged(IdProperty, oldId, newId);
                }
            }
            finally
            {
                _settingEntity = false;
            }
        }

        public override T Get<T>(IVarProperty<T> property)
        {
            if (property is IRefEntityProperty)
                return GetRefEntity((IRefEntityProperty)property).ConvertTo<T>();
            if (property is IListProperty)
                return GetLazyList((IListProperty)property).ConvertTo<T>();
            return base.Get<T>(property);
        }

        public override T Get<T>([CallerMemberName] string propertyName = null)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            var property = DoGetProperty(propertyName);
            if (property is IRefEntityProperty)
                return GetRefEntity((IRefEntityProperty)property).ConvertTo<T>();
            if (property is IListProperty)
                return GetLazyList((IListProperty)property).ConvertTo<T>();
            return base.GetValue<T>(property);
        }

        public override void Set<T>(T value, IVarProperty<T> property)
        {
            CheckSetListProperty(property);
            if (property is IRefIdProperty)
                SetRefId(value, (IRefIdProperty)property);
            else if (property is IRefEntityProperty)
                SetRefEntity(value as Entity, (IRefEntityProperty)property);
            else
                base.Set<T>(value, property);
        }

        public override void Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            var property = DoGetProperty(propertyName);
            CheckSetListProperty(property);
            if (property is IRefIdProperty)
                SetRefId(value, (IRefIdProperty)property);
            else if (property is IRefEntityProperty)
                SetRefEntity(value as Entity, (IRefEntityProperty)property);
            else
                base.SetValue(value, property);
        }

        public override object this[IVarProperty property]
        {
            get
            {
                if (property is IRefEntityProperty)
                    return GetRefEntity((IRefEntityProperty)property);
                if (property is IListProperty)
                    return GetLazyList((IListProperty)property);
                return base[property];
            }

            set
            {
                CheckSetListProperty(property);
                if (property is IRefIdProperty)
                    SetRefId(value, (IRefIdProperty)property);
                else if (property is IRefEntityProperty)
                    SetRefEntity(value as Entity, (IRefEntityProperty)property);
                else
                    base[property] = value;
            }
        }

        public override object this[string propertyName]
        {
            get
            {
                Check.NotNullOrEmpty(propertyName, nameof(propertyName));
                var property = DoGetProperty(propertyName);
                if (property is IRefEntityProperty)
                    return GetRefEntity((IRefEntityProperty)property);
                if (property is IListProperty)
                    return GetLazyList((IListProperty)property);
                return GetValue(property);
            }

            set
            {
                Check.NotNullOrEmpty(propertyName, nameof(propertyName));
                var property = DoGetProperty(propertyName);
                CheckSetListProperty(property);
                if (property is IRefIdProperty)
                    SetRefId(value, (IRefIdProperty)property);
                else if (property is IRefEntityProperty)
                    SetRefEntity(value as Entity, (IRefEntityProperty)property);
                else
                    base.SetValue(value, property);
            }
        }

        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033")]
        string IDataErrorInfo.Error
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 实体对应的仓库。
        /// </summary>
        [NonSerialized]
        IRepository _repository;

        /// <summary>
        /// 获取该实体列表对应的仓库类。
        /// </summary>
        /// <returns></returns>
        public IRepository GetRepository()
        {
            return _repository ?? (_repository = RepositoryFactory.Find(GetType()));
        }

        public void SetRefParent(Entity parent)
        {
            var property = GetRepository().GetParentProperty();
            SetRefEntity(parent, property);
        }

        #region Composition Children

        internal void SyncIdToChildren()
        {
            foreach (var field in GetLoadedChildren())
            {
                field.Value.SyncParentEntityId(this);
            }
        }

        /// <summary>
        /// 获取所有已经加载的组合子的字段集合。
        /// 
        /// 返回的字段的值必须是 IEntityOrList 类型。
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal LoadedChildrenEnumerator GetLoadedChildren()
        {
            return new LoadedChildrenEnumerator(this);
        }

        public struct LoadedChildrenEnumerator
        {
            private Entity _entity;
            private IList<IProperty> _childProperties;
            private int _index;
            private ChildPropertyField _current;

            internal LoadedChildrenEnumerator(Entity entity)
            {
                _entity = entity;
                var repo = entity.GetRepository();
                _childProperties = repo.GetChildProperties();
                _index = -1;
                _current = new ChildPropertyField();
            }

            public ChildPropertyField Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                while (true)
                {
                    _index++;
                    if (_index >= _childProperties.Count) { break; }

                    var property = _childProperties[_index];
                    var value = _entity.GetValue(property) as IEntityList;
                    if (value != null)
                    {
                        _current = new ChildPropertyField(property, value);
                        return true;
                    }
                }

                return false;
            }

            public LoadedChildrenEnumerator GetEnumerator()
            {
                //添加此方法，使得可以使用 foreach 循环
                return this;
            }
        }

        public struct ChildPropertyField
        {
            IProperty _property;
            IEntityList _value;

            public ChildPropertyField(IProperty property, IEntityList value)
            {
                _property = property;
                _value = value;
            }

            public IProperty Property
            {
                get { return _property; }
            }
            public IEntityList Value
            {
                get { return _value; }
            }
        }
        #endregion
    }
}
