using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public sealed class DbTable<T> : DbExecuter, ISubExpression
    {
        private DbTour _db;
        private ISaw _saw;
        private StringBuilderBlock _verb;
        private StringBuilderBlock _select;
        private StringBuilderBlock _from;
        private StringBuilderBlock _where;
        private StringBuilderBlock _order;
        private StringBuilderBlock _group;
        private StringBuilderBlock _having;
        private List<DbParameter> _parameters;
        private LambdaExpression _parentExpression;
        private bool _enabledAlias;

        private static IDBHelper Init(DbTour db)
        {
            Assertor.AreNull(db, "db");
            return ((IDbTourProvider)db).DBHelper;
        }
        public DbTable(DbTour db)
            : base(Init(db))
        {
            _db = db;
            _saw = ((IDbTourProvider)db).Saw ?? SqlServerSaw.Instance;
            var arr = StringBuilderBlock.Array(8);
            arr[0].Append("SELECT");
            _verb = arr[1];
            _select = arr[2];
            _from = arr[3];
            //arr[3].Append(" FROM ").Append(_saw.WarpName(SourceNameAttribute.GetName(typeof(T))));
            _where = arr[4];
            _order = arr[5];
            _group = arr[6];
            _having = arr[7];
            _parameters = new List<DbParameter>();
        }



        protected override void InitExecute()
        {
            if (_select.Length == 0)
            {
                _select.Append(" *");
                CommandText = _select.AllString();
                _select.Clear();
            }
            else
            {
                CommandText = _select.AllString();
            }
            _from.Clear();
            _from.Append(" FROM ").Append(_saw.WarpName(SourceNameAttribute.GetName(typeof(T))));
            if (_parentExpression != null)
            {
                _from.Append(' ').Append(('a' + _parentExpression.Parameters.Count) + "");
            }
            else if(_enabledAlias)
            {
                _from.Append(" a");
            }
            Parameters = _parameters.ToArray();
        }


        public DbTable<T> Where(Expression<Func<T, bool>> predicate)
        {
            var faller = Faller.Create(predicate, _parentExpression);
            _where.Append((_where.Length == 0) ? " WHERE (" : " AND (");
            _where.Append(faller.ToWhere(_saw));
            _where.Append(')');
            _parameters.AddRange(faller.Parameters);
            return this;
        }

        public DbTable<T> Or(Expression<Func<T, bool>> predicate)
        {
            var faller = Faller.Create(predicate, _parentExpression);
            _where.Append((_where.Length == 0) ? " WHERE (" : " OR (");
            _where.Append(faller.ToWhere(_saw));
            _where.Append(')');
            _parameters.AddRange(faller.Parameters);
            if (!_enabledAlias)
                _enabledAlias = faller.ExistsSubExpression;
            return this;
        }

        public DbTable<T> Distinct()
        {
            _verb.Clear();
            _verb.Append(" DISTINCT");
            return this;
        }

        public DbTable<T> GroupBy(Expression<Func<T, object>> fields)
        {
            var faller = Faller.Create(fields, _parentExpression);
            _group.Append((_group.Length == 0) ? " GROUP BY " : " ,");
            _group.Append(faller.ToSelectColumns(_saw));
            _parameters.AddRange(faller.Parameters);
            if (!_enabledAlias)
                _enabledAlias = faller.ExistsSubExpression;
            return this;
        }

        public DbTable<T> OrderBy(Expression<Func<T, object>> fields)
        {
            var faller = Faller.Create(fields, _parentExpression);
            _order.Append((_order.Length == 0) ? " ORDER BY " : " ,");
            _order.Append(faller.ToOrderBy(_saw, true));
            _parameters.AddRange(faller.Parameters);
            if (!_enabledAlias)
                _enabledAlias = faller.ExistsSubExpression;
            return this;
        }

        public DbTable<T> OrderByDescending(Expression<Func<T, object>> fields)
        {
            var faller = Faller.Create(fields, _parentExpression);
            _order.Append((_order.Length == 0) ? " ORDER BY " : " ,");
            _order.Append(faller.ToOrderBy(_saw, false));
            _parameters.AddRange(faller.Parameters);
            if (!_enabledAlias)
                _enabledAlias = faller.ExistsSubExpression;
            return this;
        }

        public DbTable<T> Skip(int count)
        {
            throw new Exception();
        }

        public DbTable<T> Take(int count)
        {
            throw new Exception();
        }

        public DbTable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            _select.Clear();
            var faller = Faller.Create(selector, _parentExpression);
            _select.Append(' ');
            _select.Append(faller.ToSelectColumns(_saw));
            _parameters.AddRange(faller.Parameters);
            if (!_enabledAlias)
                _enabledAlias = faller.ExistsSubExpression;
            return new DbTable<TResult>(_db) {
                _group = _group,
                _having = _having,
                _order = _order,
                _parameters = _parameters,
                _saw = _saw,
                _select = _select,
                _verb = _verb,
                _where = _verb,
                _parentExpression = _parentExpression,
                _enabledAlias = _enabledAlias,
            };
        }

        public TResult Sum<TResult>(Expression<Func<T, TResult>> selector)
        {
            throw new Exception();
        }

        public TResult Min<TResult>(Expression<Func<T, TResult>> selector)
        {
            throw new Exception();
        }

        public TResult Max<TResult>(Expression<Func<T, TResult>> selector)
        {
            throw new Exception();
        }

        public TResult Average<TResult>(Expression<Func<T, TResult>> selector)
        {
            throw new Exception();
        }

        public bool Exists(Expression<Func<T, bool>> predicate = null)
        {
            return false;
        }

        public int Count()
        {
            using (_verb.TemporaryArchive())
            using (_select.TemporaryArchive())
            {
                _select.Append(" COUNT(1)");
                return base.ExecuteScalar<int>();
            }
        }

        public long LongCount()
        {
            using (_verb.TemporaryArchive())
            using (_select.TemporaryArchive())
            {
                _select.Append(" COUNT(1)");
                return base.ExecuteScalar<long>();
            }
        }

        public T FirstOrDefault()
        {
            using (_verb.TemporaryArchive())
            {
                _verb.Append(" TOP 1");
                return base.FirstOrDefault<T>();
            }
        }


        LambdaExpression ISubExpression.ParentExpression
        {
            get { return _parentExpression; }
            set { _parentExpression = value; }
        }

        string ISubExpression.GetSqlString(ISawDust[] args)
        {
            return string.Concat("(", _select.AllString(), ")");
        }
    }
}
