using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Data;

namespace blqw
{
    public sealed class SqlBuilder : DbExecuter
    {
        IFQLBuilder _where;
        IFQLBuilder _order;
        IFQLBuilder _group;
        IFQLBuilder _having;
        IFQLProvider _fql;

        public SqlBuilder(DbTour tour, string sql, object[] args)
            : base(tour == null ? null : ((IDbTourProvider)tour).DBHelper)
        {
            Assertor.AreNull(sql, "sql");
            Assertor.AreNull(tour, "tour");
            if (args == null && sql.Length < 24 && sql.IndexOfAny(new char[] { ' ', '\r', '\n', '\t' }) == -1)
            {
                sql = "SELECT * FROM " + sql;
            }
            _fql = ((IDbTourProvider)tour).FQLProvider;
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
        
        protected override void InitExecute()
        {
            Executed = null;
            var p = new List<DbParameter>(_where.DbParameters);
            string a = null, b = null, c = null;
            if (_order != null && _order.IsEmpty() == false)
            {
                p.AddRange(_order.DbParameters);
                a = _order.CommandText;
                Executed += _order.ImportOutParameter;
            }
            if (_group != null && _group.IsEmpty() == false)
            {
                p.AddRange(_group.DbParameters);
                b = _group.CommandText;
                Executed += _group.ImportOutParameter;
            }
            if (_having != null && _having.IsEmpty() == false)
            {
                p.AddRange(_having.DbParameters);
                c = _having.CommandText;
                Executed += _having.ImportOutParameter;
            }
            Parameters = p.ToArray();
            CommandText = string.Concat(_where.CommandText, a, b, c);
        }


    }
}
