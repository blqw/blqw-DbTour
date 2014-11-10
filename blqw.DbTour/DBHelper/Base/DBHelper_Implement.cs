using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供操作数据库的各种功能的基类
    /// </summary>
    abstract partial class DBHelper : IDBHelper
    {
        private int _commandTimeout;
        private string _connectionString;
        private string _providerName;
        private string _name;

        /// <summary> 初始化操作
        /// </summary>
        private void Initialize()
        {
            Assertor.AreNullOrWhiteSpace(ConnectionString, "ConnectionString");
            Assertor.AreNullOrWhiteSpace(ProviderName, "ProviderName");
            ConnectorKey = string.Concat(ProviderName, "\f", ConnectionString);
            _isInitialized = true;
        }
        /// <summary> 是否已经初始化
        /// </summary>
        private bool _isInitialized;
        /// <summary> 获取数据库提供程序的工厂实例
        /// </summary>
        protected abstract DbProviderFactory Factory { get; }
        /// <summary> 获取提供访问 数据库连接 的功能的对象
        /// </summary>
        protected IConnector Connector { get; private set; }
        /// <summary> 获取用于访问 ConnectorPool 中 IConnector 的唯一标识
        /// </summary>
        protected string ConnectorKey { get; private set; }
        /// <summary> 创建新的 IConnector 对象的方法
        /// </summary>
        protected virtual IConnector GetConnector()
        {
            var conn = Factory.CreateConnection();
            conn.ConnectionString = _connectionString;
            return new Connector { DbConnection = conn };
        }
        /// <summary> 获取用于操作数据库的 DbCommand 对象实例
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        /// <returns></returns>
        protected virtual DbCommand GetCommand(CommandType commandType, string commandText, DbParameter[] parameters)
        {
            var cmd = Factory.CreateCommand();
            cmd.Connection = Connector.DbConnection;
            cmd.CommandText = commandText;
            cmd.CommandType = commandType;
            cmd.Parameters.AddRange(parameters);
            cmd.CommandTimeout = CommandTimeout;
            return cmd;
        }

        /// <summary> 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。
        /// <para>等待命令执行的时间（以秒为单位）。小于0时抛出异常</para>
        /// </summary>
        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("CommandTimeout");
                _commandTimeout = value;
            }
        }

        /// <summary> 定义数据库连接字符串值
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (_isInitialized)
                    throw new NotSupportedException("初始化完成后无法设置连接字符串");
                _connectionString = value;
            }
        }

        /// <summary> 配置文件中的连接名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_isInitialized)
                    throw new NotSupportedException("初始化完成后无法设置名称");
                _name = value;
            }
        }

        /// <summary> 数据提供程序的名称
        /// </summary>
        public string ProviderName
        {
            get { return _providerName; }
            set
            {
                if (_isInitialized)
                    throw new NotSupportedException("初始化完成后无法设置数据提供程序的名称");
                _providerName = value;
            }
        }

        /// <summary> 关闭数据库连接
        /// </summary>
        public virtual void Close()
        {
            if (Connector != null)
            {
                Connector.CloseConnection();
            }
        }

        /// <summary> 尝试打开关闭的数据库连接,如果连接已经打开,则忽略操作
        /// </summary>
        protected virtual void Open()
        {
            if (Connector == null)
            {
                if (_isInitialized == false)
                {
                    Initialize();
                }
                Connector = ConnectorPool.Get(ConnectorKey, GetConnector);
            }
            if ((Connector.DbConnection.State & ConnectionState.Open) == 0)
            {
                Connector.DbConnection.Open();
            }
        }

        /// <summary> 开启默认事务
        /// </summary>
        public abstract void Begin();

        /// <summary> 开启指定锁定行为的事务
        /// </summary>
        public abstract void Begin(IsolationLevel iso);

        /// <summary> 提交默认事务
        /// </summary>
        public abstract void Commit();

        /// <summary> 回滚默认事务
        /// </summary>
        public abstract void Rollback();

        #region Execute

        /// <summary> 按指定方式执行文本命令，并返回受影响的行数。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        public virtual int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            Open();
            using (var cmd = GetCommand(commandType, commandText, parameters))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary> 按指定方式执行文本命令，并生成 DbDataReader。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        public virtual DbDataReader ExecuteReader(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            Open();
            using (var cmd = GetCommand(commandType, commandText, parameters))
            {
                return cmd.ExecuteReader();
            }
        }

        /// <summary> 按指定方式执行文本命令，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        public virtual object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            Open();
            using (var cmd = GetCommand(commandType, commandText, parameters))
            {
                return cmd.ExecuteScalar();
            }
        }

        /// <summary> 按指定方式执行文本命令，并生成 DataTable。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        public virtual DataTable ExecuteTable(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            Open();
            using (var cmd = GetCommand(commandType, commandText, parameters))
            using (var adp = Factory.CreateDataAdapter())
            {
                adp.SelectCommand = cmd;
                var table = new DataTable();
                adp.Fill(table);
                return table;
            }
        }

        /// <summary> 按指定方式执行文本命令，并生成 DataSet。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        public virtual DataSet ExecuteSet(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            Open();
            using (var cmd = GetCommand(commandType, commandText, parameters))
            using (var adp = Factory.CreateDataAdapter())
            {
                adp.SelectCommand = cmd;
                var dataset = new DataSet();
                adp.Fill(dataset);
                return dataset;
            }
        }

        /// <summary> 执行命令，并返回受影响的行数。
        /// </summary>
        /// <param name="args">命令参数</param>
        public int ExecuteNonQuery(CommandArgs args)
        {
            if (args.CommandType == 0) args.CommandType = CommandType.Text;
            return ExecuteNonQuery(args.CommandType, args.CommandText, args.DbParameters.ToArray());
        }

        /// <summary> 执行命令，并生成 DbDataReader。
        /// </summary>
        /// <param name="args">命令参数</param>
        public DbDataReader ExecuteReader(CommandArgs args)
        {
            if (args.CommandType == 0) args.CommandType = CommandType.Text;
            return ExecuteReader(args.CommandType, args.CommandText, args.DbParameters.ToArray());
        }

        /// <summary> 执行命令，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="args">命令参数</param>
        public object ExecuteScalar(CommandArgs args)
        {
            if (args.CommandType == 0) args.CommandType = CommandType.Text;
            return ExecuteScalar(args.CommandType, args.CommandText, args.DbParameters.ToArray());
        }

        /// <summary> 执行命令，并生成 DataTable。
        /// </summary>
        /// <param name="args">命令参数</param>
        public DataTable ExecuteTable(CommandArgs args)
        {
            if (args.CommandType == 0) args.CommandType = CommandType.Text;
            return ExecuteTable(args.CommandType, args.CommandText, args.DbParameters.ToArray());
        }

        /// <summary> 执行命令，并生成 DataSet。
        /// </summary>
        /// <param name="args">命令参数</param>
        public DataSet ExecuteSet(CommandArgs args)
        {
            if (args.CommandType == 0) args.CommandType = CommandType.Text;
            return ExecuteSet(args.CommandType, args.CommandText, args.DbParameters.ToArray());
        }


        /// <summary> 执行 SQL 语句，并返回受影响的行数。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, parameters);
        }

        /// <summary> 执行 SQL 语句，并生成 DbDataReader。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        public DbDataReader ExecuteReader(string commandText, params DbParameter[] parameters)
        {
            return ExecuteReader(CommandType.Text, commandText, parameters);
        }

        /// <summary> 执行 SQL 语句，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        public object ExecuteScalar(string commandText, params DbParameter[] parameters)
        {
            return ExecuteScalar(CommandType.Text, commandText, parameters);
        }

        /// <summary> 执行 SQL 语句，并生成 DataTable。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        public DataTable ExecuteTable(string commandText, params DbParameter[] parameters)
        {
            return ExecuteTable(CommandType.Text, commandText, parameters);
        }

        /// <summary> 执行 SQL 语句，并生成 DataSet。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        public DataSet ExecuteSet(string commandText, params DbParameter[] parameters)
        {
            return ExecuteSet(CommandType.Text, commandText, parameters);
        }

        /// <summary> 不支持此功能。
        /// </summary>
        /// <param name="args"></param> 
        public IExecuteResult[] BatchExecute(params CommandArgs[] args)
        {
            throw new NotSupportedException("不支持此功能!");
        }
        #endregion

        /// <summary> 释放对于 IConnector 对象的引用
        /// </summary>
        public virtual void Dispose()
        {
            if (Connector != null)
            {
                ConnectorPool.GiveBack(ConnectorKey);
                Connector = null;
            }
        }
    }
}
