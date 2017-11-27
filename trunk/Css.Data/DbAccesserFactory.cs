using Css.Data.Transaction;
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

        /// <summary>
        /// 根据配置文件，构造一个数据库访问器。
        /// </summary>
        /// <param name="dbSettingName">数据库配置的名称。</param>
        /// <returns></returns>
        public static IDbAccesser Create(string dbSettingName)
        {
            var setting = DbSetting.FindOrCreate(dbSettingName);
            return Create(setting);
        }

        /// <summary>
        /// 根据配置文件，构造一个数据库访问器。
        /// </summary>
        /// <param name="dbSetting">数据库配置。</param>
        /// <returns></returns>
        public static IDbAccesser Create(IDbSetting dbSetting)
        {
            return new ManagedConnectionDbAccesser(dbSetting);
        }

        /// <summary>
        /// 根据数据仓储，构造一个数据库访问器。
        /// </summary>
        /// <param name="repository">仓库</param>
        /// <returns></returns>
        public static IDbAccesser Create(IDbRepository repository)
        {
            return new ManagedConnectionDbAccesser(repository.DbSetting);
        }

        #region TransactionScope
        /// <summary>
        /// 开始事务范围
        /// </summary>
        /// <param name="dbSetting"></param>
        /// <param name="level">事务的孤立级别</param>
        /// <returns></returns>
        public static SingleTransactionScope TransactionScope(string dbSetting, IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new SingleTransactionScope(DbSetting.FindOrCreate(dbSetting), level);
        }

        /// <summary>
        /// 开始事务范围
        /// </summary>
        /// <param name="dbSetting"></param>
        /// <param name="level">事务的孤立级别</param>
        /// <returns></returns>
        public static SingleTransactionScope TransactionScope(DbSetting dbSetting, IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new SingleTransactionScope(dbSetting, level);
        }

        /// <summary>
        /// 开始事务范围
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static SingleTransactionScope TransactionScope(IDbRepository repository, IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new SingleTransactionScope(repository.DbSetting, level);
        }

        /// <summary>
        /// 开始自治事务,自治事务不受嵌套影响
        /// </summary>
        /// <param name="dbSetting"></param>
        /// <param name="level">事务的孤立级别</param>
        /// <returns></returns>
        public static AutonomousTransactionScope AutonomousTransactionScope(string dbSetting, IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new AutonomousTransactionScope(DbSetting.FindOrCreate(dbSetting), level);
        }

        /// <summary>
        /// 开始自治事务,自治事务不受嵌套影响
        /// </summary>
        /// <param name="dbSetting"></param>
        /// <param name="level">事务的孤立级别</param>
        /// <returns></returns>
        public static AutonomousTransactionScope AutonomousTransactionScope(DbSetting dbSetting, IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new AutonomousTransactionScope(dbSetting, level);
        }

        /// <summary>
        /// 开始自治事务,自治事务不受嵌套影响
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static AutonomousTransactionScope AutonomousTransactionScope(IDbRepository repository, IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new AutonomousTransactionScope(repository.DbSetting, level);
        }
        #endregion
    }
}
