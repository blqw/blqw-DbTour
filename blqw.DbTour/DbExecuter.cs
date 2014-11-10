using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace blqw
{
    public class DbExecuter
    {
        public DbExecuter(IDBHelper helper)
        {
            _helper = helper;
            _commandType = CommandType.Text;
        }

        public DbExecuter(IDBHelper helper, CommandType commandType, string commandtext, DbParameter[] parameters)
        {
            _helper = helper;
            CommandText = commandtext;
            Parameters = parameters;
            _commandType = commandType;
        }

        private IDBHelper _helper;
        private CommandType _commandType;
        private DbParameter[] _parameters;
        public virtual string CommandText { get; private set; }
        public virtual DbParameter[] Parameters { get; private set; }


        protected virtual void Executed()
        {

        }


        public VarObejct GetOutValue(string name)
        {
            Assertor.AreNullOrWhiteSpace(name, "name");
            if (_parameters == null)
            {
                throw new InvalidOperationException("尚未执行查询,无法获得返回值");
            }

            var length = _parameters.Length;
            for (int i = 0; i < length; i++)
            {
                if (_parameters[i].ParameterName == name)
                {
                    return new VarObejct(_parameters[i].Value);
                }
            }
            throw new KeyNotFoundException("没有找到该名称的参数");
        }

        public DataSet ExecuteDataSet()
        {
            try
            {
                _parameters = Parameters;
                return _helper.ExecuteSet(_commandType, CommandText, _parameters);
            }
            finally
            {
                Executed();
            }
        }

        public DataTable ExecuteDataTable()
        {
            try
            {
                _parameters = Parameters;
                return _helper.ExecuteTable(_commandType, CommandText, _parameters);
            }
            finally
            {
                Executed();
            }
        }

        public void ExecuteReader(Action<IDataReader> action)
        {
            Assertor.AreNull(action, "action");
            try
            {
                _parameters = Parameters;
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, _parameters))
                {
                    action(reader);
                }
            }
            finally
            {
                Executed();
            }
        }

        public T ExecuteReader<T>(Converter<IDataReader, T> func)
        {
            Assertor.AreNull(func, "func");
            try
            {
                _parameters = Parameters;
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, _parameters))
                {
                    return func(reader);
                }
            }
            finally
            {
                Executed();
            }
        }

        public VarObejct ExecuteScalar()
        {
            try
            {
                _parameters = Parameters;
                return new VarObejct(_helper.ExecuteScalar(_commandType, CommandText, _parameters));
            }
            finally
            {
                Executed();
            }
        }

        public T ExecuteScalar<T>()
        {
            try
            {
                _parameters = Parameters;
                return (T)Convert2.ChangedType(_helper.ExecuteScalar(_commandType, CommandText, _parameters), typeof(T));
            }
            finally
            {
                Executed();
            }
        }

        public T ExecuteScalar<T>(T defaultValue)
        {
            try
            {
                _parameters = Parameters;
                return (T)Convert2.ChangedType(_helper.ExecuteScalar(_commandType, CommandText, _parameters), typeof(T), defaultValue, false);
            }
            finally
            {
                Executed();
            }
        }

        public int ExecuteNonQuery()
        {
            try
            {
                _parameters = Parameters;
                return _helper.ExecuteNonQuery(_commandType, CommandText, _parameters);
            }
            finally
            {
                Executed();
            }
        }

        public void Execute()
        {
            _parameters = Parameters;
            _helper.ExecuteNonQuery(_commandType, CommandText, _parameters);
            Executed();
        }

        public List<T> ToList<T>() where T : new()
        {
            try
            {
                _parameters = Parameters;
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, _parameters))
                {
                    return Convert2.ToList<T>(reader);
                }
            }
            finally
            {
                Executed();
            }
        }

        public List<T> ToList<T>(Converter<RowRecord, T> convert)
        {
            Assertor.AreNull(convert, "convert");
            try
            {
                _parameters = Parameters;
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, _parameters))
                {
                    var row = new RowRecord(reader, true);
                    var list = new List<T>();
                    while (reader.Read())
                    {
                        list.Add(convert(row));
                    }
                    return list;
                }
            }
            finally
            {
                Executed();
            }
        }

        public T FirstOrDefault<T>(T defaultValue = default(T)) where T : new()
        {
            try
            {
                _parameters = Parameters;
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, _parameters))
                {
                    if (reader.Read())
                    {
                        var t = new T();
                        Convert2.FillEntity(reader, ref t);
                        return t;
                    }
                    return defaultValue;
                }
            }
            finally
            {
                Executed();
            }
        }

        public T FirstOrDefault<T>(Converter<RowRecord, T> convert, T defaultValue = default(T))
        {
            Assertor.AreNull(convert, "convert");
            try
            {
                _parameters = Parameters;
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, _parameters))
                {
                    if (reader.Read())
                    {
                        return convert(new RowRecord(reader, false));
                    }
                    return defaultValue;
                }
            }
            finally
            {
                Executed();
            }
        }
    }
}
