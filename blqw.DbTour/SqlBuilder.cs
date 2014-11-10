using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Data;

namespace blqw
{
    public sealed class SqlBuilder : IExecuter
    {

        IFQLBuilder _where;
        IFQLBuilder _order;
        IFQLBuilder _group;
        IFQLBuilder _having;
        IDBHelper _helper;
        IFQLProvider _fql;

        public SqlBuilder(DbTour tour, string sql, object[] args)
        {
            Assertor.AreNull(tour, "tour");
            Assertor.AreNull(sql, "sql");
            if (args == null && sql.Length < 24 && sql.IndexOfAny(new char[] { ' ', '\r', '\n', '\t' }) == -1)
            {
                sql = "SELECT * FROM " + sql;
            }
            _helper = tour._DBHelper;
            _fql = tour._FQLProvider;
            _where = FQL.Format(_fql, sql, args).AsBuilder("WHERE");
        }

        public void And(string sql, params object[] args)
        {
            _where.And(sql, args);
        }
        public void Or(string sql, params object[] args)
        {
            _where.Or(sql, args);
        }
        public void OrderBy(string sql, params object[] args)
        {
            if (_order == null) _order = FQL.CreateBuilder(_fql, "ORDER BY");
            _order.Concat(sql, args);
        }
        public void GroupBy(string sql, params object[] args)
        {
            if (_group == null) _group = FQL.CreateBuilder(_fql, "GROUP BY");
            _group.Concat(sql, args);
        }
        public void HavingAnd(string sql, params object[] args)
        {
            if (_having == null) _having = FQL.CreateBuilder(_fql, "HAVING");
            _having.And(sql, args);
        }
        public void HavingOr(string sql, params object[] args)
        {
            if (_having == null) _having = FQL.CreateBuilder(_fql, "HAVING");
            _having.Or(sql, args);
        }

        private List<DbParameter> _parameters;
        private void Executing()
        {
            _parameters = new List<DbParameter>(_where.DbParameters);
            if (_order != null)
                _parameters.AddRange(_order.DbParameters);
            if (_group != null)
                _parameters.AddRange(_group.DbParameters);
            if (_having != null)
                _parameters.AddRange(_having.DbParameters);
        }

        private void Executed()
        {
            _where.ImportOutParameter();
            if (_order != null)
                _order.ImportOutParameter();
            if (_group != null)
                _group.ImportOutParameter();
            if (_having != null)
                _having.ImportOutParameter();
        }

        public string CommandText
        {
            get
            {
                return string.Concat(
                    _where.CommandText,
                    _order == null || _order.IsEmpty() ? null : _order.CommandText,
                    _group == null || _group.IsEmpty() ? null : _group.CommandText,
                    _having == null || _having.IsEmpty() ? null : _having.CommandText
                    );
            }
        }

        public VarObejct GetOutValue(string name)
        {
            Assertor.AreNullOrWhiteSpace(name, "name");
            if (_parameters == null)
            {
                throw new InvalidOperationException("尚未执行查询,无法获得返回值");
            }

            var p = _parameters.Find(it => it.ParameterName == name);
            if (p == null)
            {
                throw new KeyNotFoundException("没有找到该名称的参数");
            }
            return new VarObejct(p.Value);
        }

        public DataSet ExecuteDataSet()
        {
            try
            {
                Executing();
                return _helper.ExecuteSet(CommandText, _parameters.ToArray());
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
                Executing();
                return _helper.ExecuteTable(CommandText, _parameters.ToArray());
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
                Executing();
                using (var reader = _helper.ExecuteReader(CommandText, _parameters.ToArray()))
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
                Executing();
                using (var reader = _helper.ExecuteReader(CommandText, _parameters.ToArray()))
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
                Executing();
                return new VarObejct(_helper.ExecuteScalar(CommandText, _parameters.ToArray()));
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
                Executing();
                return (T)Convert2.ChangedType(_helper.ExecuteScalar(CommandText, _parameters.ToArray()), typeof(T));
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
                Executing();
                return (T)Convert2.ChangedType(_helper.ExecuteScalar(CommandText, _parameters.ToArray()), typeof(T), defaultValue, false);
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
                Executing();
                return _helper.ExecuteNonQuery(CommandText, _parameters.ToArray());
            }
            finally
            {
                Executed();
            }
        }

        public void Execute()
        {
            Executing();
            _helper.ExecuteNonQuery(CommandText, _parameters.ToArray());
            Executed();
        }

        public List<T> ToList<T>() where T : new()
        {
            try
            {
                Executing();
                using (var reader = _helper.ExecuteReader(CommandText, _parameters.ToArray()))
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
                Executing();
                using (var reader = _helper.ExecuteReader(CommandText, _parameters.ToArray()))
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
                Executing();
                using (var reader = _helper.ExecuteReader(CommandText, _parameters.ToArray()))
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
                Executing();
                using (var reader = _helper.ExecuteReader(CommandText, _parameters.ToArray()))
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
