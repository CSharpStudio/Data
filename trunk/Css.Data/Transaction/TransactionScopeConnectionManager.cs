using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.Data.Transaction
{
    /// <summary>
    /// 数据库连接的管理只是依赖当前线程中的事务。
    /// 如果代码没有在事务中时，则每次都构建新的连接，不再进行连接对象的共享。
    /// 依赖于 <see cref="LocalTransactionBlock"/> 的连接管理器。
    /// </summary>
    internal class TransactionScopeConnectionManager : IConnectionManager
    {
        private LocalTransactionBlock _block;

        private IDbConnection _connection;

        private IDbSetting _dbSetting;

        private TransactionScopeConnectionManager() { }

        public static TransactionScopeConnectionManager GetManager(IDbSetting dbSetting)
        {
            var res = new TransactionScopeConnectionManager();

            res._block = LocalTransactionBlock.GetWholeScope(dbSetting.Database);
            if (res._block != null)
            {
                res._connection = res._block.WholeTransaction.Connection;
            }
            else
            {
                //没有定义事务范围时，无需共享连接。
                res._connection = dbSetting.CreateConnection();
                res._connection.Open();
            }

            res._dbSetting = dbSetting;

            return res;
        }

        public IDbConnection Connection
        {
            get { return _connection; }
        }

        public IDbSetting DbSetting
        {
            get { return _dbSetting; }
        }

        public void Dispose()
        {
            //如果连接是来自事务，则不需要本对象来析构连接。
            if (_block == null)
            {
                _connection.Dispose();
            }
        }
    }
}
