using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data
{
    public interface IColumnInfo
    {
        /// <summary>
        /// 对应的表
        /// </summary>
        ITableInfo Table { get; }

        /// <summary>
        /// 列名
        /// </summary>
        string Name { get; }

        string PropertyName { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        Type DataType { get; }

        /// <summary>
        /// 是否为自增长主键列。
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        /// 是否可空列。
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// 是否主键列
        /// </summary>
        bool IsPrimaryKey { get; }

        /// <summary>
        /// 是否时间戳
        /// </summary>
        bool IsTimeStamp { get; }

        bool UseSequence { get; }

        object GetPropertyValue(object entity);

        void SetPropertyValue(object entity, object value);
    }
}
