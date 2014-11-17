using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public static class DbTable
    {
        public static DbTable<T> Table<T>(this DbTour db)
        {
            return new DbTable<T>(db);
        }
    }
}
