using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public partial class DBHelper
    {
        /// <summary> 创建并返回 IDBHelper,获取应用程序下ConnectionStrings中的第一个节点的值
        /// </summary>
        /// <returns></returns>
        public static IDBHelper Create()
        {
            var ee = System.Configuration.ConfigurationManager.ConnectionStrings.GetEnumerator();
            System.Configuration.ConnectionStringSettings config = null;
            while (ee.MoveNext())
            {
                config = (System.Configuration.ConnectionStringSettings)ee.Current;
                if (config.ElementInformation.IsPresent)
                {
                    break;
                }
                config = null;
            }
            if (config == null)
            {
                throw new KeyNotFoundException("不存在任何节点");
            }
            var helper = CreateDBHelper(config.ProviderName);
            helper.ConnectionString = config.ConnectionString;
            helper.ProviderName = config.ProviderName;
            helper.Name = config.Name;
            return helper;
        }
        /// <summary> 创建并返回 IDBHelper
        /// </summary>
        /// <param name="connectionName">配置节点的名称</param>
        public static IDBHelper Create(string connectionName)
        {
            Assertor.AreNullOrWhiteSpace(connectionName, "connectionName");
            var config = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName];
            if (config == null)
            {
                throw new KeyNotFoundException("不存在名为 " + connectionName + " 的节点");
            }
            var helper = CreateDBHelper(config.ProviderName);
            helper.ConnectionString = config.ConnectionString;
            helper.ProviderName = config.ProviderName;
            helper.Name = config.Name;
            return helper;
        }

        /// <summary> 创建并返回 IDBHelper
        /// </summary>
        /// <param name="connectionString">数据连接字符串</param>
        /// <param name="providerName">提供程序的名称</param>
        public static IDBHelper Create(string connectionString, string providerName)
        {
            Assertor.AreNullOrWhiteSpace(connectionString, "connectionString");
            Assertor.AreNullOrWhiteSpace(providerName, "providerName");
            var helper = CreateDBHelper(providerName);
            helper.ConnectionString = connectionString;
            helper.ProviderName = providerName;
            return helper;
        }

        /// <summary> 根据提供程序的名称创建 IDBHelper 对象
        /// </summary>
        /// <param name="providerName">提供程序名称</param>
        private static IDBHelper CreateDBHelper(string providerName)
        {
            if (providerName == null || providerName.Length == 0)
            {
                return new SqlServerHelper();
            }
            switch (providerName.ToLower())
            {
                case "sqlserver":
                case "mssql":
                case "sqlclient":
                case "system.data.sqlclient":
                    return new SqlServerHelper();
                default:
                    break;
            }
            Type type;
            if (_providers.TryGetValue(providerName, out type))
            {
                return (IDBHelper)Activator.CreateInstance(type);
            }
            lock (_providers)
            {
                if (_providers.TryGetValue(providerName, out type) == false)
                {
                    type = GetType(providerName);
                    _providers.Add(providerName, type);
                }
            }
            return (IDBHelper)Activator.CreateInstance(type);
        }
        /// <summary> 根据类型的完整名称找到 类型 对象
        /// </summary>
        /// <param name="typeFullName">类型的完整名称</param>
        private static Type GetType(string typeFullName)
        {
            var type = Type.GetType(typeFullName);
            if (type != null)
            {
                return type;
            }
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            var length = ass.Length;
            for (int i = 0; i < length; i++)
            {
                type = ass[i].GetType(typeFullName);
                if (type != null)
                {
                    return type;
                }
            }
            throw new TypeLoadException("没有找到 " + typeFullName + " 类型!请使用[命名空间.类名]的完整名称");
        }
        /// <summary> 缓存提供程序名称对应的类型对象
        /// </summary>
        private static Dictionary<string, Type> _providers = new Dictionary<string, Type>();
    }
}
