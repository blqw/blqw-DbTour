using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供对 IConnector 对象的引用计数功能
    /// </summary>
    /// <remarks>该对象并不是线程安全的</remarks>
    class SimpleCounter
    {
        /// <summary> 构造 SimpleCounter 对象的实例。
        /// </summary>
        /// <param name="connector"></param>
        public SimpleCounter(IConnector connector)
        {
            Assertor.AreNull(connector, "connection");
            Connector = connector;
        }
        /// <summary> 获取可访问数据库连接对象的实例
        /// </summary>
        public IConnector Connector { get; private set; }

        private int _referenceCount;
        /// <summary> 获取当前引用数量
        /// </summary>
        public int ReferenceCount { get { return _referenceCount; } }
        /// <summary> 增加并返回当前引用数
        /// </summary>
        public int Add()
        {
            return ++_referenceCount;
        }
        /// <summary> 减少并返回当前引用数
        /// </summary>
        /// <returns></returns>
        public int Remove()
        {
            return --_referenceCount;
        }
    }
}
