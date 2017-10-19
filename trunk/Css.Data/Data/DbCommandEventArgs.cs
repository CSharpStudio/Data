using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Css.Data
{
    /// <summary>
    /// 数据库命令事件参数
    /// </summary>
    public class DbCommandEventArgs : EventArgs
    {
        /// <summary>
        /// 获取数据库命令
        /// </summary>
        public IDbCommand DbCommand { get; }
        /// <summary>
        /// 获取范围ID，如果不为null，表示事务范围的ID
        /// </summary>
        public string ScopeId { get; }
        /// <summary>
        /// 获取命令执行失败的异常
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// 数据库命令事件参数构造器
        /// </summary>
        /// <param name="command">数据库命令</param>
        public DbCommandEventArgs(IDbCommand command)
        {
            //ScopeId = LocalTransactionBlock.GetScopeId();
            DbCommand = command;
        }
        /// <summary>
        /// 数据库命令事件参数构造器
        /// </summary>
        /// <param name="command">数据库命令</param>
        /// <param name="exc">命令执行失败的异常</param>
        public DbCommandEventArgs(IDbCommand command, Exception exc)
        {
            //ScopeId = LocalTransactionBlock.GetScopeId();
            DbCommand = command;
            Exception = exc;
        }
    }
}