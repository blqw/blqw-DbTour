using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    public static class DbTourExtension
    {
        public static DbTable<T> Table<T>(this DbTour db)
            where T : new()
        {
            return new DbTable<T>(db);
        }
    }
}
