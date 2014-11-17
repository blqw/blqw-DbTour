using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public interface IDbTourProvider
    {
        IDBHelper DBHelper { get; set; }
        IFQLProvider FQLProvider { get; set; }
        ISaw Saw { get; set; }
    }
}
