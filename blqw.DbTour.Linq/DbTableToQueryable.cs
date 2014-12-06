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
    public partial class DbTable<T> : IQueryable<T>, IQueryProvider, IDbTourProvider //, IListSource, IOrderedQueryable, IOrderedQueryable<T>,DbExecuter
    {
        private DbTour _db;

        #region IDbTourProvider
        internal IDBHelper _DBHelper;
        internal IFQLProvider _FQLProvider;
        internal ISaw _Saw;

        IDBHelper IDbTourProvider.DBHelper
        {
            get { return _DBHelper; }
        }

        IFQLProvider IDbTourProvider.FQLProvider
        {
            get { return _FQLProvider; }
        }

        ISaw IDbTourProvider.Saw
        {
            get { return _Saw; }
        } 
        #endregion

        public DbTable(DbTour tour)
        //: base(tour == null ? null : tour._DBHelper)
        {
            Assertor.AreNull(tour, "tour");
            _db = tour;
            var prov = (IDbTourProvider)tour;
            _DBHelper = prov.DBHelper;
            _FQLProvider = prov.FQLProvider;
            _Saw = prov.Saw ?? SqlServerSaw.Instance;

            Expression = Expression.Constant(this);

        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        Type _ElementType;
        public Type ElementType { get { return _ElementType ?? (_ElementType = typeof(T)); } }

        public Expression Expression { get; private set; }

        public IQueryProvider Provider
        {
            get { return this; }
        }
    }
}
