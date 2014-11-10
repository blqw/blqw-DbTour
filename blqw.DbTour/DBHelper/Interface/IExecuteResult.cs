using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供存储执行命令后的结果
    /// </summary>
    public interface IExecuteResult
    {
        /// <summary> 执行动作
        /// </summary>
        ExecuteAction Action { get; }
        /// <summary> 受影响行数
        /// </summary>
        int NonQuery { get; }
        /// <summary> DbDataReader
        /// </summary>
        DbDataReader DataReader { get; }
        /// <summary> 结果中第一行的第一列的值
        /// </summary>
        object Scalar { get; }
        /// <summary> DataTable
        /// </summary>
        DataTable DataTable { get; }
        /// <summary> DataSet
        /// </summary>
        DataSet DataSet { get; }
    }
}
