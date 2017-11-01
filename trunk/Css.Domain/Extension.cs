using Css.Data;
using Css.Domain.Query;
using Css.Domain.Query.Linq;
using Css.Reflection;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Css.Domain
{
    public static class Extension
    {
        /// <summary>
        /// 保存领域实体，通过远程服务保存，支持分布式数据提交
        /// </summary>
        /// <param name="domain"></param>
        public static void Save(this IDomain domain)
        {
            AppRuntime.Service.Resolve<EntityService>().Save(domain);
        }

        /// <summary>
        /// 指定所有属性全部映射数据库字段
        /// </summary>
        /// <param name="meta"></param>
        /// <returns></returns>
        public static EntityConfig<T> MapAllProperties<T>(this EntityConfig<T> config, params IProperty[] exceptProperties) where T : Entity
        {
            foreach (var property in config.Meta.Properties)
            {
                if (!property.IsReadonly && !(property is RefPropertyMeta) && !exceptProperties.Any(p => p.Name == property.PropertyName) && !EntityConvention.ExceptMapColumnProperties.Any(p => p.Name == property.PropertyName))
                    property.MapColumn();
            }
            return config;
        }

        /// <summary>
        /// 指定该实体类型中的某些属性直接映射数据库字段
        /// </summary>
        /// <param name="meta"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static EntityConfig<T> MapProperties<T>(this EntityConfig<T> config, params IProperty[] properties) where T : Entity
        {
            foreach (var property in properties)
            {
                var ep = config.Meta.Properties.FirstOrDefault(p => p.PropertyName == property.Name);
                if (ep == null)
                    throw new ORMException("属性[{0}]未找到对应的元数据".L10N().FormatArgs(property.Name));
                ep.MapColumn();
            }

            return config;
        }

        public static ColumnMeta MapColumn(this PropertyMeta meta)
        {
            if (meta.ColumnMeta == null)
                meta.ColumnMeta = new ColumnMeta();
            return meta.ColumnMeta;
        }

        public static PropertyMeta DontMapColumn(this PropertyMeta meta)
        {
            meta.ColumnMeta = null;
            return meta;
        }

        public static ColumnMeta HasLength(this ColumnMeta meta, string length)
        {
            meta.DataTypeLength = length;
            return meta;
        }

        public static ColumnMeta HasLength(this ColumnMeta meta, int length)
        {
            meta.DataTypeLength = length.ToString();
            return meta;
        }

        public static ColumnMeta HasColumnName(this ColumnMeta meta, string columnName)
        {
            meta.ColumnName = columnName;
            return meta;
        }

        public static ColumnMeta HasDataType(this ColumnMeta meta, DbType dataType)
        {
            meta.DataType = dataType;
            return meta;
        }

        public static ColumnMeta IsRequired(this ColumnMeta meta)
        {
            meta.IsRequired = true;
            return meta;
        }

        public static ColumnMeta IsPrimaryKey(this ColumnMeta meta, bool value)
        {
            meta.IsPrimaryKey = value;
            return meta;
        }

        public static ColumnMeta IsNullable(this ColumnMeta meta)
        {
            meta.IsRequired = false;
            return meta;
        }

        public static ColumnMeta IgnoreFK(this ColumnMeta meta)
        {
            meta.HasFKConstraint = false;
            return meta;
        }

        public static IQuery ToQuery(this IEntityQueryer queryer, out System.Linq.Expressions.NewExpression exp)
        {
            var expression = queryer.Expression;
            expression = Evaluator.PartialEval(expression);
            var builder = new EntityQueryerBuilder(queryer.Repository);
            exp = builder.NewExpression;
            var query = builder.BuildQuery(expression, queryer.Alias);
            if (!object.ReferenceEquals(queryer.WhereCriteria, null))
                query.Where = QueryFactory.Instance.And(query.Where, new CriteriaBuilder(query).Build(queryer.WhereCriteria));
            if (!object.ReferenceEquals(queryer.HavingCriteria, null))
                query.Having = QueryFactory.Instance.And(query.Having, new CriteriaBuilder(query).Build(queryer.HavingCriteria));
            return query;
        }

        public static IQuery ToQuery(this IEntityQueryer queryer)
        {
            System.Linq.Expressions.NewExpression exp;
            return ToQuery(queryer, out exp);
        }

        public static IRefEntityProperty GetParentProperty(this IRepository repo)
        {
            Debug.Assert(repo is IEntityRepository, "仓库必须是IEntityRepository");
            var entityRepo = repo as IEntityRepository;
            var result = entityRepo.ParentProperty;
            if (result == null)
                throw new ORMException("类型[{0}]没找到父引用属性".FormatArgs(repo.EntityType.Name));
            return result;
        }

        public static IProperty FindProperty(this IRepository repo, string propertyName)
        {
            Debug.Assert(repo is IEntityRepository, "仓库必须是IEntityRepository");
            var entityRepo = repo as IEntityRepository;
            return entityRepo.FindProperty(propertyName);
        }

        public static T ToObject<T>(this IDataReader reader)
        {
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return reader.GetValue(0).ConvertTo<T>();
            }
            if (typeof(T) == typeof(object))
                return reader.ToDynamic();

            var result = Activator.CreateInstance<T>();
            if (result is Entity)
            {
                var entity = result as Entity;
                var repo = RF.Find<T>();
                bool hasMeta = repo.EntityMeta.TableMeta != null;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    if (value != DBNull.Value)
                    {
                        var name = reader.GetName(i);
                        if (hasMeta)
                        {
                            var column = repo.TableInfo.Columns.FirstOrDefault(p => p.Name.CIEquals(name));
                            entity.Set(value, column.PropertyName);
                        }
                        else
                        {
                            var property = entity.PropertyContainer.Properties.FirstOrDefault(p => p.Name.CIEquals(name));
                            if (property != null)
                                entity[property] = value;
                        }
                    }
                }
                return result;
            }
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var value = reader.GetValue(i);
                if (value != DBNull.Value)
                {
                    var name = reader.GetName(i);
                    var pi = result.GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    object instance = result;
                    if (pi != null)
                        typeof(T).GetMetaAccessor(pi).SetBoxedValue(ref instance, value);

                }
            }
            return result;
        }
    }
}
