using Css.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Css.Master.EntityMetadatas
{
    [Serializable]
    public class PropertyMetadata : DataEntity
    {
        #region 名称 Name
        /// <summary>
        /// 名称
        /// </summary>
        public static readonly Property<string> NameProperty = P<PropertyMetadata>.Register(e => e.Name);
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return Get(NameProperty); }
            set { Set(value, NameProperty); }
        }
        #endregion

        #region 属性类型 PropertyType
        /// <summary>
        /// 属性类型
        /// </summary>
        public static readonly Property<string> PropertyTypeProperty = P<PropertyMetadata>.Register(e => e.PropertyType);
        /// <summary>
        /// 属性类型
        /// </summary>
        public string PropertyType
        {
            get { return Get(PropertyTypeProperty); }
            set { Set(value, PropertyTypeProperty); }
        }
        #endregion

        #region 声名类型 DeclareType
        /// <summary>
        /// 声名类型
        /// </summary>
        public static readonly Property<string> DeclareTypeProperty = P<PropertyMetadata>.Register(e => e.DeclareType);
        /// <summary>
        /// 声名类型
        /// </summary>
        public string DeclareType
        {
            get { return Get(DeclareTypeProperty); }
            set { Set(value, DeclareTypeProperty); }
        }
        #endregion

        #region 拥有者类型 OwnerType
        /// <summary>
        /// 拥有者类型
        /// </summary>
        public static readonly Property<string> OwnerTypeProperty = P<PropertyMetadata>.Register(e => e.OwnerType);
        /// <summary>
        /// 拥有者类型
        /// </summary>
        public string OwnerType
        {
            get { return Get(OwnerTypeProperty); }
            set { Set(value, OwnerTypeProperty); }
        }
        #endregion

        #region 是否序列化 Serializable
        /// <summary>
        /// 是否序列化
        /// </summary>
        public static readonly Property<bool> SerializableProperty = P<PropertyMetadata>.Register(e => e.Serializable);
        /// <summary>
        /// 是否序列化
        /// </summary>
        public bool Serializable
        {
            get { return Get(SerializableProperty); }
            set { Set(value, SerializableProperty); }
        }
        #endregion

        #region 栏位名 ColumnName
        /// <summary>
        /// 栏位名
        /// </summary>
        public static readonly Property<string> ColumnNameProperty = P<PropertyMetadata>.Register(e => e.ColumnName);
        /// <summary>
        /// 栏位名
        /// </summary>
        public string ColumnName
        {
            get { return Get(ColumnNameProperty); }
            set { Set(value, ColumnNameProperty); }
        }
        #endregion

        #region 是否自动增长 IsIdentity
        /// <summary>
        /// 是否自动增长
        /// </summary>
        public static readonly Property<bool> IsIdentityProperty = P<PropertyMetadata>.Register(e => e.IsIdentity);
        /// <summary>
        /// 是否自动增长
        /// </summary>
        public bool IsIdentity
        {
            get { return Get(IsIdentityProperty); }
            set { Set(value, IsIdentityProperty); }
        }
        #endregion

        #region 是否使用序列 UseSequence
        /// <summary>
        /// 是否使用序列
        /// </summary>
        public static readonly Property<bool> UseSequenceProperty = P<PropertyMetadata>.Register(e => e.UseSequence);
        /// <summary>
        /// 是否使用序列
        /// </summary>
        public bool UseSequence
        {
            get { return Get(UseSequenceProperty); }
            set { Set(value, UseSequenceProperty); }
        }
        #endregion

        #region 是否主键 IsPrimaryKey
        /// <summary>
        /// 是否主键
        /// </summary>
        public static readonly Property<bool> IsPrimaryKeyProperty = P<PropertyMetadata>.Register(e => e.IsPrimaryKey);
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey
        {
            get { return Get(IsPrimaryKeyProperty); }
            set { Set(value, IsPrimaryKeyProperty); }
        }
        #endregion

        #region 是否必须 IsRequired
        /// <summary>
        /// 是否必须
        /// </summary>
        public static readonly Property<bool> IsRequiredProperty = P<PropertyMetadata>.Register(e => e.IsRequired);
        /// <summary>
        /// 是否必须
        /// </summary>
        public bool IsRequired
        {
            get { return Get(IsRequiredProperty); }
            set { Set(value, IsRequiredProperty); }
        }
        #endregion

        #region 是否时间戳 IsTimeStamp
        /// <summary>
        /// 是否时间戳
        /// </summary>
        public static readonly Property<bool> IsTimeStampProperty = P<PropertyMetadata>.Register(e => e.IsTimeStamp);
        /// <summary>
        /// 是否时间戳
        /// </summary>
        public bool IsTimeStamp
        {
            get { return Get(IsTimeStampProperty); }
            set { Set(value, IsTimeStampProperty); }
        }
        #endregion

        #region 数据类型 DataType
        /// <summary>
        /// 数据类型
        /// </summary>
        public static readonly Property<string> DataTypeProperty = P<PropertyMetadata>.Register(e => e.DataType);
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType
        {
            get { return Get(DataTypeProperty); }
            set { Set(value, DataTypeProperty); }
        }
        #endregion

        #region 数据长度 DataTypeLength
        /// <summary>
        /// 数据长度
        /// </summary>
        public static readonly Property<string> DataTypeLengthProperty = P<PropertyMetadata>.Register(e => e.DataTypeLength);
        /// <summary>
        /// 数据长度
        /// </summary>
        public string DataTypeLength
        {
            get { return Get(DataTypeLengthProperty); }
            set { Set(value, DataTypeLengthProperty); }
        }
        #endregion

        #region 是否有外键 HasFKConstraint
        /// <summary>
        /// 是否有外键
        /// </summary>
        public static readonly Property<bool> HasFKConstraintProperty = P<PropertyMetadata>.Register(e => e.HasFKConstraint);
        /// <summary>
        /// 是否有外键
        /// </summary>
        public bool HasFKConstraint
        {
            get { return Get(HasFKConstraintProperty); }
            set { Set(value, HasFKConstraintProperty); }
        }
        #endregion

        #region 引用类型 ReferenceType
        /// <summary>
        /// 引用类型
        /// </summary>
        public static readonly Property<ReferenceType?> ReferenceTypeProperty = P<PropertyMetadata>.Register(e => e.ReferenceType);
        /// <summary>
        /// 引用类型
        /// </summary>
        public ReferenceType? ReferenceType
        {
            get { return Get(ReferenceTypeProperty); }
            set { Set(value, ReferenceTypeProperty); }
        }
        #endregion

        #region 引用属性名称 RefPropertyName
        /// <summary>
        /// 引用属性名称
        /// </summary>
        public static readonly Property<string> RefPropertyNameProperty = P<PropertyMetadata>.Register(e => e.RefPropertyName);
        /// <summary>
        /// 引用属性名称
        /// </summary>
        public string RefPropertyName
        {
            get { return Get(RefPropertyNameProperty); }
            set { Set(value, RefPropertyNameProperty); }
        }
        #endregion

        #region 引用属性类型 RefPropertyType
        /// <summary>
        /// 引用属性类型
        /// </summary>
        public static readonly Property<string> RefPropertyTypeProperty = P<PropertyMetadata>.Register(e => e.RefPropertyType);
        /// <summary>
        /// 引用属性类型
        /// </summary>
        public string RefPropertyType
        {
            get { return Get(RefPropertyTypeProperty); }
            set { Set(value, RefPropertyTypeProperty); }
        }
        #endregion
    }

    class PropertyMetadataConfig : EntityConfig<PropertyMetadata>
    {
        protected override void ConfigMeta()
        {
            MapTable("SYS_ENTITY_PROP_META").MapAllProperties();
        }
    }
}
