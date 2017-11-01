namespace Css.Domain
{
    /// <summary>
    /// 引用属性元数据
    /// </summary>
    public class RefPropertyMeta : PropertyMeta
    {
        /// <summary>
        /// 引用类型
        /// </summary>
        public ReferenceType ReferenceType { get; set; }

        /// <summary>
        /// 引用
        /// </summary>
        public PropertyInfoMeta RefProperty { get; set; }

        /// <summary>
        /// 是否可空
        /// </summary>
        public bool Nullable { get; set; }
    }
}
