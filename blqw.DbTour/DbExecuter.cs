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
            Assertor.AreNull(helper, "IDBHelper");
            _helper = helper;
            _commandType = CommandType.Text;
        }

        public DbExecuter(IDBHelper helper, CommandType commandType, string commandtext, DbParameter[] parameters, System.Threading.ThreadStart executed)
        {
            Assertor.AreNull(helper, "IDBHelper");
            _helper = helper;
            CommandText = commandtext;
            Parameters = parameters;
            _commandType = commandType;
            Executed = executed;
        }

        private IDBHelper _helper;
        private CommandType _commandType;
        public string CommandText { get; protected set; }
        public DbParameter[] Parameters { get; protected set; }

        protected System.Threading.ThreadStart Executed;

        protected virtual void InitExecute()
        { 
            
        }

        protected virtual void OnExecuted()
        {
            if (Executed != null)
            {
                Executed();
            }
        }

        public VarObejct GetOutValue(string name)
        {
            Assertor.AreNullOrWhiteSpace(name, "name");
            if (Parameters == null)
            {
                throw new InvalidOperationException("尚未执行查询,无法获得返回值");
            }

            var length = Parameters.Length;
            for (int i = 0; i < length; i++)
            {
                if (string.Equals(Parameters[i].ParameterName, name, StringComparison.OrdinalIgnoreCase))
                {
                    return new VarObejct(Parameters[i].Value);
                }
            }
            throw new KeyNotFoundException("没有找到该名称的参数");
        }

        public DataSet ExecuteDataSet()
        {
            try
            {
                InitExecute();
                return _helper.ExecuteSet(_commandType, CommandText, Parameters);
            }
            finally
            {
                OnExecuted();
            }
        }

        public DataTable ExecuteDataTable()
        {
            try
            {
                InitExecute();
                return _helper.ExecuteTable(_commandType, CommandText, Parameters);
            }
            finally
            {
                OnExecuted();
            }
        }

        public void ExecuteReader(Action<IDataReader> action)
        {
            Assertor.AreNull(action, "action");
            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
                {
                    action(reader);
                }
            }
            finally
            {
                OnExecuted();
            }
        }

        public T ExecuteReader<T>(Converter<IDataReader, T> func)
        {
            Assertor.AreNull(func, "func");
            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
                {
                    return func(reader);
                }
            }
            finally
            {
                OnExecuted();
            }
        }

        public VarObejct ExecuteScalar()
        {
            try
            {
                InitExecute();
                return new VarObejct(_helper.ExecuteScalar(_commandType, CommandText, Parameters));
            }
            finally
            {
                OnExecuted();
            }
        }

        public T ExecuteScalar<T>()
        {
            try
            {
                InitExecute();
                return (T)Convert2.ChangedType(_helper.ExecuteScalar(_commandType, CommandText, Parameters), typeof(T));
            }
            finally
            {
                OnExecuted();
            }
        }

        public T ExecuteScalar<T>(T defaultValue)
        {
            try
            {
                InitExecute();
                return (T)Convert2.ChangedType(_helper.ExecuteScalar(_commandType, CommandText, Parameters), typeof(T), defaultValue, false);
            }
            finally
            {
                OnExecuted();
            }
        }

        public int ExecuteNonQuery()
        {
            try
            {
                InitExecute();
                return _helper.ExecuteNonQuery(_commandType, CommandText, Parameters);
            }
            finally
            {
                OnExecuted();
            }
        }

        public void Execute()
        {
            InitExecute();
            _helper.ExecuteNonQuery(_commandType, CommandText, Parameters);
            OnExecuted();
        }

        public List<T> ToList<T>() where T : new()
        {
            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
                {
                    return Convert2.ToList<T>(reader);
                }
            }
            finally
            {
                OnExecuted();
            }
        }

        public List<T> ToList<T>(Converter<RowRecord, T> convert)
        {
            Assertor.AreNull(convert, "convert");
            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
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
                OnExecuted();
            }
        }

        public T FirstOrDefault<T>(T defaultValue = default(T))
        {
            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
                {
                    if (reader.Read())
                    {

                        T t = default(T);
                        if (Convert2.FillEntity(reader, ref t))
                        {
                            return t;
                        }
                    }
                    return defaultValue;
                }
            }
            finally
            {
                OnExecuted();
            }
        }

        public T FirstOrDefault<T>(Converter<RowRecord, T> convert, T defaultValue = default(T))
        {
            Assertor.AreNull(convert, "convert");
            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
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
                OnExecuted();
            }
        }

        public List<object> ToList()
        {
            if (DynamicType == null)
            {
                InitDynamicType();
            }

            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
                {
                    List<object> list = new List<object>();
                    var length = reader.FieldCount;
                    string[] name = new string[length];

                    if (reader.Read())
                    {
                        var model = (IDictionary<string, object>)Activator.CreateInstance(DynamicType);
                        for (int i = 0; i < length; i++)
                        {
                            model.Add(name[i] = reader.GetName(i), new VarObejct(reader[i]));
                        }
                        list.Add(model);
                    }
                    while (reader.Read())
                    {
                        var model = (IDictionary<string, object>)Activator.CreateInstance(DynamicType);
                        for (int i = 0; i < length; i++)
                        {
                            model.Add(name[i], new VarObejct(reader[i]));
                        }
                        list.Add(model);
                    }
                    return list;
                }
            }
            finally
            {
                OnExecuted();
            }
        }

        public object FirstOrDefault(object defaultValue = null)
        {
            if (DynamicType == null)
            {
                InitDynamicType();
            }
            try
            {
                InitExecute();
                using (var reader = _helper.ExecuteReader(_commandType, CommandText, Parameters))
                {
                    if (reader.Read())
                    {
                        var model = (IDictionary<string, object>)Activator.CreateInstance(DynamicType);
                        var length = reader.FieldCount;
                        for (int i = 0; i < length; i++)
                        {
                            model.Add(reader.GetName(i), new VarObejct(reader[i]));
                        }
                        return model;
                    }
                    return defaultValue;
                }
            }
            finally
            {
                OnExecuted();
            }
        }

        #region 动态类型
        static Type DynamicType;
        static void InitDynamicType()
        {
            DynamicType = Type.GetType("System.Dynamic.ExpandoObject, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            if (DynamicType != null)
            {
                return;
            }
            var ass = AppDomain.CurrentDomain.GetAssemblies();
            var length = ass.Length;
            for (int i = 0; i < length; i++)
            {
                if (ass[i].GetName().Name == "System.Core")
                {
                    DynamicType = ass[i].GetType("System.Dynamic.ExpandoObject");
                    break;
                }
            }
            if (DynamicType == null)
            {
                throw new TypeLoadException("dynamic类型加载失败!");
            }
        } 
        #endregion
    }
}
