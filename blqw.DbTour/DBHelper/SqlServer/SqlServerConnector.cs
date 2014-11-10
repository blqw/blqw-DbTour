using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace blqw
{
    /// <summary> 提供访问和操作 SQL Server 数据库的部分功能
    /// </summary>
    class SqlServerConnector : IConnector
    {
        /// <summary> 构造 SqlServerConnector 对象
        /// </summary>
        /// <param name="connectionString">SQL Server 数据库的连接字符串</param>
        public SqlServerConnector(string connectionString)
        {
            DbConnection = new SqlConnection(connectionString);
        }
        /// <summary> 获取 SQL Server 数据库的连接对象
        /// </summary>
        public SqlConnection DbConnection { get; private set; }
        /// <summary> 获取当前 SQL Server 数据库连接中的事务
        /// </summary>
        public SqlTransaction Transaction { get; private set; }
        /// <summary> 开启事务或保存点,返回string.Empty或保存点的名称
        /// </summary>
        public string BeginTransaction()
        {
            if (Transaction == null)
            {
                Transaction = DbConnection.BeginTransaction();
                return string.Empty;
            }
            else
            {
                var tranPoint = Guid.NewGuid().ToString("N");
                Transaction.Save(tranPoint);
                return tranPoint;
            }
        }
        /// <summary> 开启指定级别的事务或保存点,返回string.Empty或保存点的名称
        /// </summary>
        public string BeginTransaction(IsolationLevel iso)
        {
            if (Transaction == null)
            {
                Transaction = DbConnection.BeginTransaction(iso);
                return string.Empty;
            }
            else if ((Transaction.IsolationLevel & iso) != 0)
            {
                var tranPoint = "tran_" + Guid.NewGuid().ToString("N");
                Transaction.Save(tranPoint);
                return tranPoint;
            }
            else
            {
                throw new NotSupportedException("子事务不能更改锁定级别");
            }
        }
        /// <summary> 回滚到指定的保存点
        /// </summary>
        /// <param name="tranPoint">保存点的名称</param>
        public void Rollback(string tranPoint)
        {
            if (tranPoint == null)
            {
                return;
            }
            if (Transaction == null)
            {
                return;
            }
            if (tranPoint.Length == 0)
            {
                Transaction.Rollback();
                Transaction = null;
            }
            else
            {
                Transaction.Rollback(tranPoint);
            }
        }

        /// <summary> 提交指定的保存点的修改
        /// </summary>
        /// <param name="tranPoint">保存点的名称</param>
        public void Commit(string tranPoint)
        {
            if (tranPoint != null && tranPoint.Length == 0 && Transaction != null)
            {
                Transaction.Commit();
                Transaction = null;
            }
        }

        /// <summary> 关闭数据库连接
        /// </summary>
        public void CloseConnection()
        {
            DbConnection.Close();
        }
        /// <summary> SqlConnection
        /// </summary>
        DbConnection IConnector.DbConnection { get { return DbConnection; } }
    }
}
