using Css.ComponentModel;
using Css.Data;
using Css.Data.Common;
using Css.Diagnostics;
using Css.Domain.Query;
using Css.Domain.Query.Impl;
using Css.Domain.Query.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.Domain
{
    public abstract class EntityRepository : IEntityRepository, IDbRepository
    {
        /// <summary>
        /// 这个字段用于存储运行时解析出来的 ORM 信息。
        /// </summary>
        IDbTable _dbTable;
        public IDbTable DbTable
        {
            get { return _dbTable ?? (_dbTable = DbManager.Manager.CreateDbTable(DbSetting, TableInfo)); }
        }

        IDbSetting _dbSetting;
        protected IDbSetting DbSetting
        {
            get { return _dbSetting ?? (_dbSetting = DbManager.Manager.GetDbSetting(ConnectionStringName)); }
        }

        IDbSetting IDbRepository.DbSetting
        {
            get { return DbSetting; }
        }

        ITableInfo _tableInfo;
        public ITableInfo TableInfo
        {
            get { return _tableInfo ?? (_tableInfo = Mapping.TableInfo.Create(EntityMeta)); }
        }

        public abstract Type EntityType { get; }
        public abstract Type EntityListType { get; }

        EntityMeta _entityMeta;
        public EntityMeta EntityMeta
        {
            get { return _entityMeta ?? (_entityMeta = EntityMetaRepository.Instance.Find(EntityType)); }
        }

        VarPropertyContainer _propertyContainer;
        public VarPropertyContainer PropertyContainer
        {
            get { return _propertyContainer ?? (_propertyContainer = VarTypeRepository.Instance.GetVarPropertyContainer(EntityType)); }
        }

        public abstract string ConnectionStringName { get; }

        /// <summary>
        /// 创建数据库操作对象
        /// </summary>
        /// <returns></returns>
        public IDbAccesser CreateDbAccesser()
        {
            return DbManager.Manager.CreateDbAccesser(DbSetting);
        }

        public virtual IEntity GetById(object id)
        {
            var f = QueryFactory.Instance;
            var table = f.Table(this);
            var query = f.Query(
                table,
                where: f.Constraint(table.IdColumn, id)
            );
            query.MainTable.Alias = QueryGenerationContext.Get(query).NextTableAlias();
            return FirstOrDefault(query);
        }

        public virtual IList GetByParentId(object parentId)
        {
            var f = QueryFactory.Instance;
            var parentProperty = EntityMeta.FindParentProperty();

            var table = f.Table(this);
            var query = f.Query(
                table,
                where: f.Constraint(table.Column(parentProperty.PropertyName), parentId)
            );
            query.MainTable.Alias = QueryGenerationContext.Get(query).NextTableAlias();
            return ToList(query);
        }

        public void Save(IDomain component)
        {
            if (component is Entity)
                Save(component as Entity);
            else
                Save(component as IEntityList);
        }

        protected virtual void Save(IEntityList list)
        {
            foreach (Entity item in list.Deleted)
            {
                Save(item);
            }
            list.Deleted.Clear();
            foreach (Entity item in list)
            {
                if (item.DataState != DataState.Normal)
                    Save(item);
            }
        }

        protected virtual void Save(Entity entity)
        {
            //创建提交数据的参数。
            var e = new SubmitArgs
            {
                Entity = entity,
                Action = GetAction(entity)
            };
            Submit(e);
            switch (e.Action)
            {
                case SubmitAction.Delete:
                    entity.MarkCreated();
                    break;
                case SubmitAction.Update:
                case SubmitAction.Insert:
                case SubmitAction.ChildrenOnly:
                    entity.ResetState();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        SubmitAction GetAction(Entity entity)
        {
            if (entity.DataState == DataState.Deleted)
                return SubmitAction.Delete;
            if (entity.DataState == DataState.Created)
                return SubmitAction.Insert;
            if (entity.DataState == DataState.Modified)
                return SubmitAction.Update;
            return SubmitAction.ChildrenOnly;
        }

        protected virtual void Submit(SubmitArgs e)
        {
            using (EventTracer.Start("Submit", e))
            {
                using (EventTracer.Start("Validate", e.Entity))
                {
                    //var broken = e.Entity.Validate(ValidatorActions.None);
                    //if (broken.Count > 0)
                    //    throw new ValidationException(broken.ToString());
                }
                if (!RepositoryFactory.OnSubmitting(e)) return;

                var entity = e.Entity;
                switch (e.Action)
                {
                    case SubmitAction.Delete:
                        Delete(entity);
                        break;
                    case SubmitAction.Insert:
                        Insert(entity);
                        break;
                    case SubmitAction.Update:
                        Update(entity);
                        break;
                    case SubmitAction.ChildrenOnly:
                        SubmitChildren(entity);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                RepositoryFactory.OnSubmitted(e);
            }
        }

        protected virtual void SubmitChildren(Entity entity)
        {
            var children = entity.GetLoadedChildren();
            foreach (var child in children)
            {
                var c = child.Value;
                c.GetRepository().Save(c);
            }
        }

        protected virtual void Insert(Entity entity)
        {
            if (!RepositoryFactory.OnInserting(entity)) return;
            using (var dba = CreateDbAccesser())
            {
                DbTable.Insert(dba, entity);
            }
        }

        protected virtual void Delete(Entity entity)
        {
            if (!RepositoryFactory.OnDeleting(entity)) return;
            using (var dba = CreateDbAccesser())
            {
                DbTable.Delete(dba, entity);
            }
        }

        protected virtual void Update(Entity entity)
        {
            if (!RepositoryFactory.OnUpdating(entity)) return;
            using (var dba = CreateDbAccesser())
            {
                DbTable.Update(dba, entity);
            }
        }

        public long GetNextId()
        {
            using (var dba = CreateDbAccesser())
            {
                var sql = DbTable.SqlDialect.SelectSeqNextValueSql(DbTable.Name, DbTable.PKColumn.Name);
                return dba.ExecuteScalar(sql).ConvertTo<long>();
            }
        }

        public int Count(IQuery query)
        {
            using (var dba = CreateDbAccesser())
            {
                query.IsCounting = true;
                var generator = DbTable.CreateSqlGenerator();
                generator.Generate(query as TableQuery);
                return dba.ExecuteScalar(generator.Sql, generator.Sql.Parameters).ConvertTo<int>();
            }
        }

        public IEntityList ToList(IQuery query, int start = 0, int end = 0)
        {
            using (var dba = CreateDbAccesser())
            {
                var generator = DbTable.CreateSqlGenerator();
                generator.Generate(query as TableQuery, start, end);
                using (var reader = dba.ExecuteReader(generator.Sql, dba.Connection.State == ConnectionState.Closed, generator.Sql.Parameters))
                {
                    var result = Activator.CreateInstance(EntityListType) as IEntityList;
                    while (reader.Read())
                    {
                        var e = Activator.CreateInstance(EntityType) as Entity;
                        ReadData(reader, e);
                        result.Add(e);
                    }
                    if (start != 0 || end != 0)
                        result.Total = Count(query);

                    return result;
                }
            }
        }

        public IEntity FirstOrDefault(IQuery query)
        {
            using (var dba = CreateDbAccesser())
            {
                var generator = DbTable.CreateSqlGenerator();
                generator.Generate(query as TableQuery);
                using (var reader = dba.ExecuteReader(generator.Sql, dba.Connection.State == ConnectionState.Closed, generator.Sql.Parameters))
                {
                    if (reader.Read())
                    {
                        var result = Activator.CreateInstance(EntityType) as Entity;
                        ReadData(reader, result);
                        return result;
                    }
                    return null;
                }
            }
        }

        void ReadData(SafeDataReader reader, Entity entity)
        {
            foreach (var column in TableInfo.Columns)
            {
                var value = reader.GetValue(column.Name);
                column.SetPropertyValue(entity, value);
            }
        }

        public async Task<IEntity> GetByIdAsync(object id)
        {
            return await Task.Run(() => GetById(id));
        }

        public async Task SaveAsync(IDomain entity)
        {
            await Task.Run(() => Save(entity));
        }

        public async Task<int> CountAsync(IQuery query)
        {
            return await Task.Run(() => Count(query));
        }

        public async Task<IEntityList> ToListAsync(IQuery query, int start, int end)
        {
            return await Task.Run(() => ToList(query, start, end));
        }

        public async Task<IEntity> FirstOrDefaultAsync(IQuery query)
        {
            return await Task.Run(() => FirstOrDefault(query));
        }
    }

    public class EntityRepository<T> : EntityRepository where T : Entity
    {
        string _connectionStringName;
        Type _entityType;
        Type _entityListType;

        public EntityRepository()
        {
            _entityType = typeof(T);
        }

        public override string ConnectionStringName
        {
            get { return _connectionStringName ?? (_connectionStringName = GetConnectionStringName()); }
        }

        public override Type EntityType
        {
            get { return _entityType; }
        }

        public override Type EntityListType
        {
            get { return _entityListType ?? (_entityListType = RF.Host.ConventionListForEntity(EntityType)); }
        }

        string GetConnectionStringName()
        {
            var attrConnection = EntityType.Assembly.GetCustomAttribute<ConnectionForRepositoryAttribute>();
            return attrConnection?.ConnectionStringName ?? "";
        }


    }
}
