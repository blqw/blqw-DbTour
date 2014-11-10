using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace blqw
{
    /// <summary> 数据库行
    /// </summary>
    public sealed class RowRecord
    {
        /// <summary> 获取
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IDictionary<string, int> GetColumnsMap(IDataReader reader)
        {
            var length = reader.FieldCount;
            var cols = new Dictionary<string, int>(length, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < length; i++)
            {
                cols[reader.GetName(i)] = i;
            }
            return cols;
        }

        IDataReader _reader;
        IDictionary<string, int> _cols;

        public RowRecord(IDataReader reader, bool cacheColumns)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (cacheColumns)
            {
                _cols = GetColumnsMap(reader);
            }
            _reader = reader;
            CheckClosed();
        }
        
        /// <summary> 获取位于指定索引处的列的值。
        /// </summary>
        /// <param name="index">要获取的列的从零开始的索引。</param> 
        public VarObejct this[int index]
        {
            get
            {
                CheckClosed();
                if (index < 0)
                {
                    throw new IndexOutOfRangeException("索引不能小于0");
                }
                else if (index > _reader.FieldCount - 1)
                {
                    return new VarObejct(null);
                }
                else
                {
                    return new VarObejct(_reader[index]);
                }
            }
        }

        /// <summary> 获取具有指定名称的列的值。
        /// </summary>
        /// <param name="name">要查找的列的名称。</param> 
        public VarObejct this[string name]
        {
            get
            {
                CheckClosed();
                if (string.IsNullOrEmpty(name) || (name = name.Trim()).Length == 0)
                {
                    throw new IndexOutOfRangeException("列名不能为空");
                }
                int index = GetIndex(name);
                if (index == -1)
                {
                    return new VarObejct(null);
                }
                return new VarObejct(_reader[index]);
            }
        }

        /// <summary> 转为实体对象
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        public T To<T>()
        {
            CheckClosed();
            var lit = Literacy.Cache(typeof(T), true);
            var obj = lit.NewObject();
            if (_cols != null)
            {
                foreach (var col in _cols)
                {
                    var p = lit.Property[col.Key];
                    p.TrySetValue(obj, ChangeType(_reader[col.Value], p.OriginalType));
                }
            }
            else
            {
                foreach (var p in lit.Property)
                {
                    var index = _reader.GetOrdinal(p.Name);
                    if (index > -1)
                    {
                        p.TrySetValue(obj, ChangeType(_reader[index], p.OriginalType));
                    }
                }
            }
            return (T)obj;
        }

        #region 私有
        /// <summary> 类型转换
        /// </summary>
        private object ChangeType(object obj, Type type)
        {
            return Convert2.ChangedType(obj, type);
        }

        /// <summary> 根据name获取index
        /// </summary>
        private int GetIndex(string name)
        {
            CheckClosed();
            if (_cols != null)
            {
                int index;
                if (_cols.TryGetValue(name, out index))
                {
                    return index;
                }
                return -1;
            }
            else
            {
                try
                {
                    return _reader.GetOrdinal(name);
                }
                catch (Exception)
                {
                    var length = _reader.FieldCount;
                    var dict = new Dictionary<string, int>(length, StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < length; i++)
                    {
                        dict[_reader.GetName(i)] = i;
                    }
                    _cols = dict;
                }
            }
            return GetIndex(name);
        }

        /// <summary> 如果_reader已经关闭,抛出异常
        /// </summary>
        private void CheckClosed()
        {
            if (_reader.IsClosed)
            {
                throw new InvalidOperationException("DataReader已经关闭");
            }
        }
        #endregion
    }
}
