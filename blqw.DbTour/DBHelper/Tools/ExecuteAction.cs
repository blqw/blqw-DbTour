using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    /// <summary> 指定对执行命令动作的说明
    /// </summary>
    public enum ExecuteAction
    {
        /// <summary> 执行 ExecuteNonQuery, 返回 受影响行数
        /// </summary>
        NonQuery = 1,
        /// <summary> 执行 ExecuteReader, 返回 DbDataReader
        /// </summary>
        DbDataReader = 2,
        /// <summary> 执行 ExecuteScalar,返回第一行第一列
        /// </summary>
        Scalar = 3,
        /// <summary> 执行 ExecuteTable,返回 DataTable
        /// </summary>
        DataTable = 4,
        /// <summary> 执行 ExecuteDataSet,返回 DataSet
        /// </summary>
        DataSet = 5,
    }
}
