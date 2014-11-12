using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace blqw
{
    public sealed class DbTour : IDisposable
    {
        //public DbTour()
        //{
        //    var configs = System.Configuration.ConfigurationManager.ConnectionStrings;
        //    if (configs.Count == 0)
        //    {
        //        throw new NotSupportedException("缺少配置信息");
        //    }
        //    var name = configs[0].Name;
        //    _DBHelper = DBHelper.Create(name);
        //    Initialize();
        //}
        public DbTour(string connectionName)
        {
            _DBHelper = DBHelper.Create(connectionName);
            Initialize();
        }


        public DbTour(string connectionString, string providerName)
        {
            _DBHelper = DBHelper.Create(connectionString, providerName);
            Initialize();
        }

        private void Initialize()
        {
            Assertor.AreNull(_DBHelper, "DBHelper");
            var factory = _DBHelper as IDbComponentFactory;
            if (factory == null)
            {
                if (_DBHelper is SqlServerHelper)
                {
                    _FQLProvider = SqlServerFQL.Instance;
                    _Saw = null;
                    return;
                }
                throw new NotSupportedException(TypesHelper.DisplayName(_DBHelper.GetType()) + " 没有实现 IDbComponentFactory 接口");
            }
            _FQLProvider = factory.CreateFQLProvider();
            _Saw = factory.CreateSaw();
        }

        internal IDBHelper _DBHelper;
        internal IFQLProvider _FQLProvider;
        internal ISaw _Saw;
        #region config
        /// <summary> 定义数据库连接字符串值
        /// </summary>
        public string ConnectionString { get { return _DBHelper.ConnectionString; } }
        /// <summary> 配置文件中的连接名称
        /// </summary>
        public string Name { get { return _DBHelper.Name; } }
        /// <summary> 数据提供程序的名称
        /// </summary>
        public string ProviderName { get { return _DBHelper.ProviderName; } }
        #endregion

        /// <summary> 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。
        /// <para>等待命令执行的时间（以秒为单位）。小于0时抛出异常</para>
        /// </summary>
        public int CommandTimeout
        {
            get { return _DBHelper.CommandTimeout; }
            set { _DBHelper.CommandTimeout = value; }
        }
        /// <summary> 关闭数据库连接
        /// </summary>
        public void Close()
        {
            _DBHelper.Close();
        }

        /// <summary> 设置sql语句和参数,得到执行器
        /// </summary>
        /// <param name="commandText">sql语句</param>
        /// <param name="args">参数</param>
        public DbExecuter Sql(string commandText, params object[] args)
        {
            var fql = FQL.Format(_FQLProvider, commandText, args);
            return new DbExecuter(_DBHelper, CommandType.Text, fql.CommandText, fql.DbParameters, fql.ImportOutParameter);
        }
        /// <summary> 设置存储过程名称和参数,得到执行器
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="args">参数</param>
        public DbExecuter Proc(string procedureName, params object[] args)
        {
            if (args == null)
            {
                return new DbExecuter(_DBHelper, CommandType.StoredProcedure, procedureName, new DbParameter[0], null);
            }
            var length = args.Length;
            DbParameter[] parameters = new DbParameter[length];
            for (int i = 0; i < length; i++)
            {
                var p = args[i] as DbParameter;
                parameters[i] = (p == null) ? _FQLProvider.CreateDbParameter(args[i]) : p;
            }
            return new DbExecuter(_DBHelper, CommandType.StoredProcedure, procedureName, parameters, null);
        }


        #region transaction
        /// <summary> 开启默认事务
        /// </summary>
        public void Begin()
        {
            _DBHelper.Begin();
        }
        /// <summary> 开启指定锁定行为的事务
        /// </summary>
        /// <param name="il">事务的锁定行为</param>
        public void Begin(IsolationLevel iso)
        {
            _DBHelper.Begin();
        }
        /// <summary> 提交默认事务
        /// </summary>
        public void Commit()
        {
            _DBHelper.Commit();
        }
        /// <summary> 回滚默认事务
        /// </summary>
        public void Rollback()
        {
            _DBHelper.Rollback();
        }
        #endregion

        public void Dispose()
        {
            var helper = _DBHelper;
            if (helper != null)
            {
                _DBHelper.Dispose();
                _DBHelper = null;
            }
        }
    }
}
