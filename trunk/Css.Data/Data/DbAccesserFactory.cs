using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Css.Data
{
    /// <summary>
    /// 关系数据库访问器工厂
    /// </summary>
    public class DbAccesserFactory
    {
        /// <summary>
        /// 数据库命令执行失败的事件
        /// </summary>
        public static event EventHandler<DbCommandEventArgs> Error;

        /// <summary>
        /// 数据库命令准备完成的事件
        /// </summary>
        public static event EventHandler<DbCommandEventArgs> DbCommandPrepared;

        internal static void OnError(IDbAccesser dba, IDbCommand command, Exception exc)
        {
            Error?.Invoke(dba, new DbCommandEventArgs(command, exc));
        }

        internal static void OnDbCommandPrepared(IDbAccesser dba, IDbCommand command)
        {
            DbCommandPrepared?.Invoke(dba, new DbCommandEventArgs(command));
        }
    }
}
