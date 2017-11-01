using Css.Data;
using Css.Data.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain.Mapping
{
    public class TableInfo : ITableInfo
    {
        public TableInfo()
        {
            Columns = new List<ColumnInfo>();
        }

        /// <summary>
        /// 对应的实体类型
        /// </summary>
        public Type Class { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 主键列（每个表肯定有一个主键列）
        /// </summary>
        public ColumnInfo PKColumn { get; set; }

        /// <summary>
        /// 所有的列
        /// </summary>
        public List<ColumnInfo> Columns { get; set; }

        public string ViewSql { get; set; }

        IColumnInfo ITableInfo.PKColumn { get { return PKColumn; } }

        IReadOnlyList<IColumnInfo> ITableInfo.Columns { get { return Columns; } }

        public static ITableInfo Create(EntityMeta meta)
        {
            Check.NotNull(meta, nameof(meta));
            if (meta.TableMeta == null)
                throw new ORMException("类型[{0}]没有映射数据库".FormatArgs(meta.EntityType.FullName));
            var table = new TableInfo();
            table.Class = meta.EntityType;
            table.Name = meta.TableMeta.TableName;
            table.ViewSql = meta.TableMeta.ViewSql;
            foreach (var property in meta.Properties)
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
    }
}
