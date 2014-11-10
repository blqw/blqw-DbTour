using System;
using System.Collections.Generic;
using System.Data;

namespace blqw
{
    /// <summary> 执行器基类用于执行sql语句,获得返回值
    /// </summary>
    public interface IExecuter
    {
        /// <summary> 获取数据源运行的文本命令。
        /// </summary>
        string CommandText { get; }

        /// <summary> 获取返回参数的值
        /// </summary>
        /// <param name="name">参数名</param>
        VarObejct GetOutValue(string name);
        

        /// <summary> 执行指令,返回DataSet
        /// </summary>
        DataSet ExecuteDataSet();
        /// <summary> 执行指令,返回DataTable
        /// </summary>
        DataTable ExecuteDataTable();


        /// <summary> 执行指令,在委托中操作DataReader
        /// </summary>
        void ExecuteReader(Action<IDataReader> action);
        /// <summary> 执行指令,在委托中操作DataReader,并返回结果
        /// </summary>
        T ExecuteReader<T>(Converter<IDataReader, T> func);


        /// <summary> 执行指令,返回第一行第一列的值
        /// </summary>
        VarObejct ExecuteScalar();
        /// <summary> 执行指令,返回第一行第一列的强类型,转换失败抛出异常
        /// </summary>
        T ExecuteScalar<T>();
        /// <summary> 执行指令,返回第一行第一列的强类型,转换失败返回默认值
        /// </summary>
        /// <param name="defaultValue">转换失败时候返回的默认值</param>
        T ExecuteScalar<T>(T defaultValue);

        /// <summary> 执行指令,返回受影响行数
        /// </summary>
        /// <returns></returns>
        int ExecuteNonQuery();


        void Execute();


        /// <summary> 执行指令,返回实体集合
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        List<T> ToList<T>() where T : new();
        /// <summary> 执行指令,根据委托转换实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="convert">转换委托</param>
        List<T> ToList<T>(Converter<RowRecord, T> convert);
        /// <summary> 执行指令,返回第一行的数据,并转换为实体对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        T FirstOrDefault<T>(T defaultValue = default(T)) where T : new();
        /// <summary> 执行指令,返回第一行的数据,根据委托转换实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="convert">转换委托</param>
        T FirstOrDefault<T>(Converter<RowRecord, T> convert, T defaultValue = default(T));

    }
}
