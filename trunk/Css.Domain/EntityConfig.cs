using Css.ComponentModel;
using Css.Domain.Metadata;
using Css.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public class EntityConfig<T> : IEntityConfig where T : Entity
    {
        EntityMeta _meta;

        EntityMeta IEntityConfig.Meta
        {
            get { return _meta; }
            set { _meta = value; }
        }

        protected internal EntityMeta Meta { get { return _meta; } }

        public EntityConfig<T> MapTable(string tableName)
        {
            Check.NotNullOrEmpty(tableName, nameof(tableName));
            Meta.TableMeta = new TableMeta { TableName = tableName };
            MapIdColumn(Meta);
            return this;
        }

        public EntityConfig<T> MapView(string sql)
        {
            Check.NotNullOrEmpty(sql, nameof(sql));
            Meta.TableMeta = new TableMeta { ViewSql = sql };
            MapIdColumn(Meta);
            return this;
        }

        static void MapIdColumn(EntityMeta meta)
        {
            var idProperty = VarTypeRepository.Instance.GetVarPropertyContainer(meta.EntityType).Properties.FirstOrDefault(p => p.Name == Entity.IdProperty.Name);
            var id = meta.Properties.FirstOrDefault(p => p.PropertyName == idProperty.Name);
            id.ColumnMeta = new ColumnMeta
            {
                ColumnName = id.PropertyName.ToUpper(),
                IsPrimaryKey = true,
            };
            //int和long默认使用自增长,Oracle中使用序列
            if (idProperty.PropertyType == typeof(int) || idProperty.PropertyType == typeof(long))
                id.ColumnMeta.IsIdentity = true;
            //double型ID主要用作分布式数据,整数部分使用序列,小数部分区分数据库
            if (idProperty.PropertyType == typeof(double))
                id.ColumnMeta.UseSequence = true;
        }

        public PropertyMeta Property(Expression<Func<T, object>> propertyExp)
        {
            Check.NotNull(propertyExp, nameof(propertyExp));
            return Meta.Properties.FirstOrDefault(p => p.PropertyName == Reflect<T>.GetProperty(propertyExp).Name);
        }

        public PropertyMeta Property(IProperty property)
        {
            Check.NotNull(property, nameof(property));
            return Meta.Properties.FirstOrDefault(p => p.PropertyName == property.Name);
        }

        public PropertyMeta Property(string propertyName)
        {
            Check.NotNullOrEmpty(propertyName, nameof(propertyName));
            return Meta.Properties.FirstOrDefault(p => p.PropertyName == propertyName);
        }

        public Type EntityType { get { return typeof(T); } }

        protected virtual void ConfigMeta()
        {

        }

        void IEntityConfig.ConfigMeta()
        {
            ConfigMeta();
        }
    }
}
