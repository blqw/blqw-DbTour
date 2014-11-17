using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blqw
{
    internal class DbTourProvider : IDbTourProvider
    {
        public IDBHelper DBHelper { get; set; }

        public IFQLProvider FQLProvider { get; set; }

        public ISaw Saw { get; set; }
    }
}
