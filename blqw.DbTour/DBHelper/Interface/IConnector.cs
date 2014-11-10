using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供访问 数据库连接 的功能
    /// </summary>
    public interface IConnector
    {
        /// <summary> 数据库连接对象
        /// </summary>
        DbConnection DbConnection { get; }
        /// <summary> 关闭数据库连接
        /// </summary>
        /// <remarks>该方法在一般情况下不应抛出异常</remarks>
        void CloseConnection();
    }
}
