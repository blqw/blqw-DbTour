using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace blqw
{
    /// <summary> 提供执行数据库命令时所需要的参数
    /// </summary>
    public struct CommandArgs
    {
        /// <summary> 要执行的文本命令
        /// </summary>
        public string CommandText { get; set; }
        /// <summary> 指示或指定如何解释 CommandText 属性
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary> 执行文本命令的参数
        /// </summary>
        public List<DbParameter> DbParameters { get; set; }
    }
}
