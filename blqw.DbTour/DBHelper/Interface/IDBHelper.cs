using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供操作数据库的各种功能
    /// </summary>
    public interface IDBHelper : IDisposable
    {
        #region config
        /// <summary> 定义数据库连接字符串值
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary> 配置文件中的连接名称
        /// </summary>
        string Name { get; set; }
        /// <summary> 数据提供程序的名称
        /// </summary>
        string ProviderName { get; set; }
        #endregion

        /// <summary> 获取或设置在终止执行命令的尝试并生成错误之前的等待时间。
        /// <para>等待命令执行的时间（以秒为单位）。小于0时抛出异常</para>
        /// </summary>
        int CommandTimeout { get; set; }
        /// <summary> 关闭数据库连接
        /// </summary>
        void Close();

        #region transaction
        /// <summary> 开启默认事务
        /// </summary>
        void Begin();
        /// <summary> 开启指定锁定行为的事务
        /// </summary>
        /// <param name="il">事务的锁定行为</param>
        void Begin(IsolationLevel iso);
        /// <summary> 提交默认事务
        /// </summary>
        void Commit();
        /// <summary> 回滚默认事务
        /// </summary>
        void Rollback();
        #endregion

        #region execute
        /// <summary> 执行 SQL 语句，并返回受影响的行数。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        int ExecuteNonQuery(string commandText, params DbParameter[] parameters);
        /// <summary> 执行 SQL 语句，并生成 DbDataReader。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        DbDataReader ExecuteReader(string commandText, params DbParameter[] parameters);
        /// <summary> 执行 SQL 语句，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        object ExecuteScalar(string commandText, params DbParameter[] parameters);
        /// <summary> 执行 SQL 语句，并生成 DataTable。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        DataTable ExecuteTable(string commandText, params DbParameter[] parameters);
        /// <summary> 执行 SQL 语句，并生成 DataSet。
        /// </summary>
        /// <param name="commandText">要执行的 SQL 语句</param>
        /// <param name="parameters">SQL 语句的参数</param>
        DataSet ExecuteSet(string commandText, params DbParameter[] parameters);

        /// <summary> 按指定方式执行文本命令，并返回受影响的行数。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] parameters);
        /// <summary> 按指定方式执行文本命令，并生成 DbDataReader。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        DbDataReader ExecuteReader(CommandType commandType, string commandText, params DbParameter[] parameters);
        /// <summary> 按指定方式执行文本命令，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] parameters);
        /// <summary> 按指定方式执行文本命令，并生成 DataTable。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        DataTable ExecuteTable(CommandType commandType, string commandText, params DbParameter[] parameters);
        /// <summary> 按指定方式执行文本命令，并生成 DataSet。
        /// </summary>
        /// <param name="commandType">指示或指定如何解释 commandText 参数</param>
        /// <param name="commandText">要执行的文本命令</param>
        /// <param name="parameters">执行文本命令的参数</param>
        DataSet ExecuteSet(CommandType commandType, string commandText, params DbParameter[] parameters);

        /// <summary> 执行命令，并返回受影响的行数。
        /// </summary>
        /// <param name="args">命令参数</param>
        int ExecuteNonQuery(CommandArgs args);
        /// <summary> 执行命令，并生成 DbDataReader。
        /// </summary>
        /// <param name="args">命令参数</param>
        DbDataReader ExecuteReader(CommandArgs args);
        /// <summary> 执行命令，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="args">命令参数</param>
        object ExecuteScalar(CommandArgs args);
        /// <summary> 执行命令，并生成 DataTable。
        /// </summary>
        /// <param name="args">命令参数</param>
        DataTable ExecuteTable(CommandArgs args);
        /// <summary> 执行命令，并生成 DataSet。
        /// </summary>
        /// <param name="args">命令参数</param>
        DataSet ExecuteSet(CommandArgs args);

        /// <summary> 批量执行命令，并返回结果。
        /// </summary>
        /// <param name="args">命令参数集合</param> 
        IExecuteResult[] BatchExecute(params CommandArgs[] args); 
        #endregion
    }
}
