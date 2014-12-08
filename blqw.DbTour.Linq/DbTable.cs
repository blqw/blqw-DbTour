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
        private StringBuilderBlock _where;
        private StringBuilderBlock _order;
        private StringBuilderBlock _group;
        private StringBuilderBlock _having;
        private List<DbParameter> _parameters;

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
            arr[3].Append(" FROM ").Append(_saw.WarpName(SourceNameAttribute.GetName(typeof(T))));
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
            Parameters = _parameters.ToArray();
        }


        public DbTable<T> Where(Expression<Func<T, bool>> predicate)
        {
            var faller = Faller.Create(predicate);
            _where.Append((_where.Length == 0) ? " WHERE (" : " AND (");
            _where.Append(faller.ToWhere(_saw));
            _where.Append(')');
            _parameters.AddRange(faller.Parameters);
            return this;
        }

        public DbTable<T> Or(Expression<Func<T, bool>> predicate)
        {
            var faller = Faller.Create(predicate);
            _where.Append((_where.Length == 0) ? " WHERE (" : " OR (");
            _where.Append(faller.ToWhere(_saw));
            _where.Append(')');
            _parameters.AddRange(faller.Parameters);
            return this;
        }

        public DbTable<T> Distinct()
        {
            _verb.Append(" DISTINCT");
            return this;
        }

        public DbTable<T> GroupBy(Expression<Func<T, object>> fields)
        {
            var faller = Faller.Create(fields);
            _group.Append((_where.Length == 0) ? " GROUP BY " : " ,");
            _group.Append(faller.ToSelectColumns(_saw));
            _parameters.AddRange(faller.Parameters);
            return this;
        }

        public DbTable<T> OrderBy(Expression<Func<T, object>> fields)
        {
            var faller = Faller.Create(fields);
            _order.Append((_where.Length == 0) ? " ORDER BY " : " ,");
            _order.Append(faller.ToOrderBy(_saw, true));
            _parameters.AddRange(faller.Parameters);
            return this;
        }

        public DbTable<T> OrderByDescending(Expression<Func<T, object>> fields)
        {
            var faller = Faller.Create(fields);
            _order.Append((_where.Length == 0) ? " ORDER BY " : " ,");
            _order.Append(faller.ToOrderBy(_saw, false));
            _parameters.AddRange(faller.Parameters);
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
            var faller = Faller.Create(selector);
            _where.Append((_where.Length == 0) ? " ORDER BY " : " ,");
            _where.Append(faller.ToSelectColumns(_saw));
            _parameters.AddRange(faller.Parameters);
            return new DbTable<TResult>(_db) {
                _group = _group,
                _having = _having,
                _order = _order,
                _parameters = _parameters,
                _saw = _saw,
                _select = _select,
                _verb = _verb,
                _where = _verb,
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

        LambdaExpression _ParentExpression;

        LambdaExpression ISubExpression.ParentExpression
        {
            get { return _ParentExpression; }
            set { _ParentExpression = value; }
        }

        string ISubExpression.GetSqlString(ISawDust[] args)
        {
            return string.Concat("(", _select.AllString(), ")");
        }
    }
}
