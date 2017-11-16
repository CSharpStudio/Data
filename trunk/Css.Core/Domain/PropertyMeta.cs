using Css.Data;

namespace Css.Domain
{
    /// <summary>
    /// 属性元数据，表示非聚合或组合子的属性
    /// </summary>
    public class PropertyMeta : PropertyMetadata
    {
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadonly { get; set; }

        /// <summary>
        /// 数据列元数据
        /// </summary>
        public ColumnMeta ColumnMeta { get; set; }
    }
}
