using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供管理当前线程数据库连接的连接池
    /// </summary>
    static class ConnectorPool
    {
        /// <summary> 提供保存当前线程的连接对象
        /// </summary>
        [ThreadStatic]
        public static Dictionary<string, SimpleCounter> Connections;

        /// <summary> 根据key,在当前线程中获取一个唯一的数据库连接,并增加引用数,如果key对应的连接不存在 则使用get获得
        /// </summary>
        /// <param name="key">连接的唯一标识符</param>
        /// <param name="get">获取连接的委托</param>
        public static IConnector Get(string key, GetConnectionHandler get)
        {
            SimpleCounter counter;
            if (Connections == null)
            {
                Connections = new Dictionary<string, SimpleCounter>();
            }
            else if (Connections.TryGetValue(key, out counter))
            {
                counter.Add();
                return counter.Connector;
            }
            var conn = get();
            if (conn != null && conn.DbConnection != null)
            {
                counter = new SimpleCounter(conn);
                Connections.Add(key, counter);
                counter.Add();
            }
            return conn;
        }

        /// <summary> 归还key所表示的连接,引用数为0时将尝试关闭连接
        /// </summary>
        /// <param name="key"></param>
        public static void GiveBack(string key)
        {
            SimpleCounter counter;
            if (Connections != null && Connections.TryGetValue(key, out counter))
            {
                if (counter.Remove() == 0)
                {
                    counter.Connector.CloseConnection();
                }
            }
        }
    }
}
