using Css.Domain;
using System;

namespace Css.Master.EntityMetadatas
{
    [RootEntity, Serializable]
    public class EntityMetadata : DataEntity
    {
        #region 名称 Name
        /// <summary>
        /// 名称
        /// </summary>
        public static readonly Property<string> NameProperty = P<EntityMetadata>.Register(e => e.Name);
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return Get(NameProperty); }
            set { Set(value, NameProperty); }
        }
        #endregion

        #region 命名空间 Namespace
        /// <summary>
        /// 命名空间
        /// </summary>
        public static readonly Property<string> NamespaceProperty = P<EntityMetadata>.Register(e => e.Namespace);
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace
        {
            get { return Get(NamespaceProperty); }
            set { Set(value, NamespaceProperty); }
        }
        #endregion

        #region 程序集 Assembly
        /// <summary>
        /// 程序集
        /// </summary>
        public static readonly Property<string> AssemblyProperty = P<EntityMetadata>.Register(e => e.Assembly);
        /// <summary>
        /// 程序集
        /// </summary>
        public string Assembly
        {
            get { return Get(AssemblyProperty); }
            set { Set(value, AssemblyProperty); }
        }
        #endregion
        
        #region 属性列表 PropertyMetadata
        /// <summary>
        /// 属性列表
        /// </summary>
        public static readonly ListProperty<EntityList<PropertyMetadata>> PropertyMetadataListProperty = P<EntityMetadata>.RegisterList(e => e.PropertyMetadataList);
        /// <summary>
        /// 属性列表
        /// </summary>
        public EntityList<PropertyMetadata> PropertyMetadataList
        {
            get { return GetLazyList(PropertyMetadataListProperty); }
        }
        #endregion

        #region 表名 TableName
        /// <summary>
        /// 表名
        /// </summary>
        public static readonly Property<string> TableNameProperty = P<EntityMetadata>.Register(e => e.TableName);
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return Get(TableNameProperty); }
            set { Set(value, TableNameProperty); }
        }
        #endregion

        #region 基类 BaseType
        /// <summary>
        /// 基类
        /// </summary>
        public static readonly Property<string> BaseTypeProperty = P<EntityMetadata>.Register(e => e.BaseType);
        /// <summary>
        /// 基类
        /// </summary>
        public string BaseType
        {
            get { return Get(BaseTypeProperty); }
            set { Set(value, BaseTypeProperty); }
        }
        #endregion
        
        #region 是否视图 IsView
        /// <summary>
        /// 是否视图
        /// </summary>
        public static readonly Property<bool> IsViewProperty = P<EntityMetadata>.Register(e => e.IsView);
        /// <summary>
        /// 是否视图
        /// </summary>
        public bool IsView
        {
            get { return Get(IsViewProperty); }
            set { Set(value, IsViewProperty); }
        }
        #endregion
    }

    class EntityMetadataConfig : EntityConfig<EntityMetadata>
    {
        protected override void ConfigMeta()
        {
            MapTable("SYS_ENTITY_META").MapAllProperties();
        }
    }
}
