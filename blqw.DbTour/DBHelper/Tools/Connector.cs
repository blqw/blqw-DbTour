using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供访问和修改 数据库连接对象 的功能
    /// </summary>
    class Connector : IConnector
    {
        /// <summary> 数据库连接对象
        /// </summary>
        public DbConnection DbConnection { get; set; }

        /// <summary> 关闭数据库连接
        /// </summary>
        public void CloseConnection()
        {
            if (DbConnection != null)
            {
                DbConnection.Close();
            }
        }
    }
}
