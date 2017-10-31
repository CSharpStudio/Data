using Css.ComponentModel;
using Css.Data.Common;
using Css.Domain.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Metadata
{
    public class EntityMeta
    {
        /// <summary>
        /// 当前模型是对应这个类型的。
        /// </summary>
        public Type EntityType { get; set; }

        public TableMeta TableMeta { get; set; }

        public IList<PropertyMeta> Properties { get; } = new List<PropertyMeta>();

        public IList<ChildPropertyMeta> ChildrenProperties { get; } = new List<ChildPropertyMeta>();

        protected internal ITableInfo CreateTableInfo()
        {
            if (TableMeta == null)
                throw new ORMException("类型[{0}]没有映射数据库".FormatArgs(EntityType.FullName));
            var table = new TableInfo();
            table.Class = EntityType;
            table.Name = TableMeta.TableName;
            table.ViewSql = TableMeta.ViewSql;
            foreach (var property in Properties)
            {
                var cm = property.ColumnMeta;
                if (cm == null) continue;
                var column = new ColumnInfo
                {
                    Table = table,
                    PropertyName = property.PropertyName,
                    DataType = property.PropertyType,
                    Name = property.ColumnMeta.ColumnName ?? property.PropertyName,
                    IsIdentity = cm.IsIdentity,
                    UseSequence = cm.UseSequence,
                    IsPrimaryKey = cm.IsPrimaryKey,
                    IsTimeStamp = cm.IsTimeStamp,
                };
                if (cm.IsPrimaryKey)
                    table.PKColumn = column;
                table.Columns.Add(column);
            }
            return table;
        }

        public RefPropertyMeta FindParentProperty()
        {
            return Properties.OfType<RefPropertyMeta>().FirstOrDefault(p => p.ReferenceType == ReferenceType.Parent);
        }
    }
}
