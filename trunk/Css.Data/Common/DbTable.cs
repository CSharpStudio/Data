using Css.Data.SqlTree;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    public abstract class DbTable
    {
        ITableInfo _tableInfo;

        List<DbColumn> _columns = new List<DbColumn>();

        public string Name { get { return _tableInfo.Name; } }

        public string ViewSql { get { return _tableInfo.ViewSql; } }

        /// <summary>
        /// 主键列。
        /// </summary>
        public DbColumn PKColumn { get; protected set; }
        /// <summary>
        /// 时间戳，某些表会没有
        /// </summary>
        public DbColumn TimeStampColumn { get; protected set; }

        public IReadOnlyList<DbColumn> Columns
        {
            get { return _columns; }
        }

        public abstract SqlGenerator CreateSqlGenerator();

        public abstract ISqlDialect SqlDialect { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214")]
        public DbTable(ITableInfo info)
        {
            Check.NotNull(info, nameof(info));
            _tableInfo = info;
            foreach (var column in info.Columns)
            {
                Add(CreateColumn(column));
            }
        }

        protected virtual DbColumn CreateColumn(IColumnInfo columnInfo)
        {
            return new DbColumn(this, columnInfo);
        }

        protected virtual void Add(DbColumn column)
        {
            if (column.Info.IsPrimaryKey)
            {
                if (PKColumn != null)
                    throw new DataException("cannot add primary key column {0} to table {1}, it already has primary key column {2}".FormatArgs(column.Name, Name, PKColumn.Name));

                PKColumn = column;
            }

            if (column.Info.IsTimeStamp)
            {
                if (TimeStampColumn != null)
                    throw new DataException("cannot add timestamp column {0} to table {1}, it already has timestamp column {2}".FormatArgs(column.Name, Name, TimeStampColumn.Name));
                TimeStampColumn = column;
            }

            _columns.Add(column);
        }

        protected enum ReadDataType { ByIndex, ByName }


        void EnsureMappingTable()
        {
            if (!ViewSql.IsNullOrEmpty()) throw new NotSupportedException("{0} 类映射视图，不能进行 CDU 操作。".FormatArgs(_tableInfo.Class.Name));
        }

        string _insertSql, _deleteSql, _updateSql;

        /// <summary>
        /// 执行 sql 插入一个实体到数据库中。
        /// 基类的默认实现中，只是简单地实现了 sql 语句的生成和执行。
        /// </summary>
        /// <param name="dba"></param>
        /// <param name="item"></param>
        public virtual void Insert(IDbAccesser dba, object item)
        {
            EnsureMappingTable();
            if (_insertSql == null) { _insertSql = GenerateInsertSql(); }
            var parameters = _columns.Where(p => p.CanInsert).Select(p => p.GetValue(item));
            dba.ExecuteNonQuery(_insertSql, parameters.ToArray());
        }

        protected virtual string GenerateInsertSql()
        {
            var sql = new StringWriter();
            sql.Write("INSERT INTO ");
            sql.Write(SqlDialect.PrepareIdentifier(Name));
            sql.Write(" (");
            var values = new StringBuilder();
            bool comma = false;
            var index = 0;
            foreach (var column in Columns)
            {
                if (column.CanInsert)
                {
                    if (comma)
                    {
                        sql.Write(',');
                        values.Append(',');
                    }
                    else { comma = true; }

                    sql.Write(SqlDialect.PrepareIdentifier(column.Name));
                    values.Append('{').Append(index++).Append('}');
                }
            }
            sql.Write(") VALUES (");
            sql.Write(values.ToString());
            sql.Write(")");
            return sql.ToString();
        }

        public int Delete(IDbAccesser dba, object item)
        {
            EnsureMappingTable();
            if (_deleteSql == null) { _deleteSql = GenerateDeleteSql(); }
            return dba.ExecuteNonQuery(_deleteSql, PKColumn.GetValue(item));
        }

        public int Delete(IDbAccesser dba, ISqlConstraint where)
        {
            Check.NotNull(where, nameof(where));
            EnsureMappingTable();
            var sql = new StringWriter();
            sql.Write("DELETE FROM ");
            sql.Write(SqlDialect.PrepareIdentifier(Name));
            sql.Write(" WHERE ");
            var generator = CreateSqlGenerator();
            generator.Generate(where as SqlNode);
            var whereSql = generator.Sql;
            sql.Write(whereSql.ToString());
            return dba.ExecuteNonQuery(sql.ToString(), whereSql.Parameters);
        }

        protected virtual string GenerateDeleteSql()
        {
            var sql = new StringWriter();
            sql.Write("DELETE FROM ");
            sql.Write(SqlDialect.PrepareIdentifier(Name));
            sql.Write(" WHERE ");
            sql.Write(SqlDialect.PrepareIdentifier(PKColumn.Name));
            sql.Write(" = {0}");
            return sql.ToString();
        }

        public int Update(IDbAccesser dba, object item)
        {
            EnsureMappingTable();
            if (_updateSql == null) { _updateSql = GenerateUpdateSql(); }
            List<object> parameters = new List<object>(Columns.Count);
            foreach (var column in Columns)
            {
                if (!column.Info.IsPrimaryKey && !column.Info.IsTimeStamp)
                    parameters.Add(column.GetValue(item));
            }
            if (TimeStampColumn != null)
                parameters.Add(DateTime.Now.Ticks);
            parameters.Add(PKColumn.GetValue(item));
            if (TimeStampColumn != null)
                parameters.Add(TimeStampColumn.GetValue(item));
            return dba.ExecuteNonQuery(_updateSql, parameters.ToArray());
        }

        protected virtual string GenerateUpdateSql()
        {
            var sql = new StringWriter();
            sql.Write("UPDATE ");
            sql.Write(SqlDialect.PrepareIdentifier(Name));
            sql.Write(" SET ");
            bool comma = false;
            var paramIndex = 0;
            foreach (var column in Columns)
            {
                if (!column.Info.IsPrimaryKey && !column.Info.IsTimeStamp)
                {
                    if (comma) { sql.Write(','); }
                    else { comma = true; }
                    sql.Write(SqlDialect.PrepareIdentifier(column.Name));
                    sql.Write(" = {");
                    sql.Write(paramIndex++);
                    sql.Write('}');
                }
            }
            if (TimeStampColumn != null)
            {
                if (comma) { sql.Write(','); }
                sql.Write(SqlDialect.PrepareIdentifier(TimeStampColumn.Name));
                sql.Write(" = {");
                sql.Write(paramIndex++);
                sql.Write('}');
            }
            sql.Write(" WHERE ");
            sql.Write(SqlDialect.PrepareIdentifier(PKColumn.Name));
            sql.Write(" = {");
            sql.Write(paramIndex++);
            sql.Write('}');
            if (TimeStampColumn != null)
            {
                sql.Write(" AND ");
                sql.Write(SqlDialect.PrepareIdentifier(TimeStampColumn.Name));
                sql.Write(" = {");
                sql.Write(paramIndex++);
                sql.Write('}');
            }
            return sql.ToString();
        }

        public int Execute(IDbAccesser dba, ExecuteArgs args)
        {
            EnsureMappingTable();
            var sql = args.Type == ExecuteType.Update ? GenerateUpdateSql(args) : GenerateDeleteSql(args);
            return dba.ExecuteNonQuery(sql.ToString(), sql.Parameters.ToArray());
        }

        FormattedSql GenerateDeleteSql(ExecuteArgs item)
        {
            var sql = new FormattedSql();
            sql.Append("DELETE FROM ");
            sql.Append(SqlDialect.PrepareIdentifier(Name));
            var generator = CreateSqlGenerator();
            generator.Generate(item.Where as SqlNode);
            var where = generator.Sql;
            foreach (var p in where.Parameters.ToArray())
                sql.Parameters.Add(SqlDialect.PrepareValue(p));
            sql.Append(" WHERE ");
            sql.Append(where.ToString());
            return sql;
        }

        FormattedSql GenerateUpdateSql(ExecuteArgs item)
        {
            var generator = CreateSqlGenerator();
            var sql = generator.Sql;
            sql.Append("UPDATE ");
            sql.Append(SqlDialect.PrepareIdentifier(Name));
            sql.Append(" SET ");
            bool comma = false;
            foreach (var c in item.Columns)
            {
                var column = Columns.FirstOrDefault(p => p.Info.PropertyName == c.PropertyName);
                if (!column.Info.IsPrimaryKey)
                {
                    if (comma) { sql.Append(','); }
                    else { comma = true; }
                    sql.Append(SqlDialect.PrepareIdentifier(column.Name));
                    if (c.Value is SqlNode)
                    {
                        sql.Append(" = ");
                        generator.Generate(c.Value as SqlNode);
                    }
                    else
                    {
                        sql.Append(" = {");
                        sql.Append(sql.Parameters.Count);
                        sql.Append('}');
                        if (column.Info.IsTimeStamp)
                            sql.Parameters.Add(DateTime.Now.Ticks);
                        else
                            sql.Parameters.Add(SqlDialect.PrepareValue(c.Value));
                    }
                }
            }
            sql.Append(" WHERE ");
            generator.Generate(item.Where as SqlNode);
            return sql;
        }
    }
}
