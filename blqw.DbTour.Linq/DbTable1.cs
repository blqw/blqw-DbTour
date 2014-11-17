using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Collections;

namespace blqw
{
    public class DbTable<T> : IQueryable<T>, IQueryProvider //, IListSource, IOrderedQueryable, IOrderedQueryable<T>
    {
        private DbTour _db;
        private DbTourProvider _prov;

        public DbTable(DbTour db)
        {
            _db = db;
            Expression = Expression.Constant(this);
            _prov = new DbTourProvider();
            _db.TransProvider(_prov);
            if (_prov.Saw == null)
            {
                _prov.Saw = SqlServerSaw.Instance;
            }
        }



        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Type ElementType
        {
            get { throw new NotImplementedException(); }
        }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider
        {
            get { return this; }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var expr = expression as LambdaExpression;
            if (expr == null)
            {
                throw new NotSupportedException("expression不是有效的LambdaExpression对象");
            }
            var faller = Faller.Create(expr);
            var sql = faller.ToWhere(_prov.Saw);
            Console.WriteLine(sql);
            return new DbTable<TElement>(_db);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<T>(Expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
