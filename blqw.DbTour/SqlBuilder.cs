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

        private static IDBHelper GetDbHelper(DbTour tour)
        {
            Assertor.AreNull(tour, "tour");
            return tour._DBHelper;
        }

        public SqlBuilder(DbTour tour, string sql, object[] args)
            : base(GetDbHelper(tour))
        {
            Assertor.AreNull(sql, "sql");
            if (args == null && sql.Length < 24 && sql.IndexOfAny(new char[] { ' ', '\r', '\n', '\t' }) == -1)
            {
                sql = "SELECT * FROM " + sql;
            }
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

        public override DbParameter[] Parameters
        {
            get
            {
                var p = new List<DbParameter>(_where.DbParameters);
                if (_order != null)
                    p.AddRange(_order.DbParameters);
                if (_group != null)
                    p.AddRange(_group.DbParameters);
                if (_having != null)
                    p.AddRange(_having.DbParameters);
                return p.ToArray();
            }
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

    }
}
