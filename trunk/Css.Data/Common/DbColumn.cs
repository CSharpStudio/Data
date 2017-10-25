using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Common
{
    [DebuggerDisplay("Name:{Name}")]
    public class DbColumn
    {
        DbTable _table;
        IColumnInfo _columnInfo;

        protected internal DbColumn(DbTable table, IColumnInfo columnInfo)
        {
            _table = table;
            _columnInfo = columnInfo;
        }

        /// <summary>
        /// 列名
        /// </summary>
        public string Name
        {
            get { return _columnInfo.Name; }
        }

        public DbTable Table
        {
            get { return _table; }
        }

        /// <summary>
        /// 列的信息
        /// </summary>
        public IColumnInfo Info
        {
            get { return _columnInfo; }
        }

        public virtual bool CanInsert
        {
            get
            {
                //Sql Server 中的 Identity 列是不需要插入的。
                return !_columnInfo.IsIdentity;
            }
        }

        /// <summary>
        /// 读取实体中本列对应的属性的值，该值将被写入到数据库中对应的列。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object GetValue(object entity)
        {
            object value = Info.GetPropertyValue(entity);
            return Table.SqlDialect.PrepareValue(value);
        }

        /// <summary>
        /// 把数据库中列的值写入到实体对应的属性中。
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="val"></param>
        public void SetValue(object entity, object val)
        {
            if (val == DBNull.Value) { val = null; }
            Info.SetPropertyValue(entity, val);
        }
    }
}
