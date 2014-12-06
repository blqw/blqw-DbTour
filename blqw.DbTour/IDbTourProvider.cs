using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public interface IDbTourProvider
    {
        IDBHelper DBHelper { get;}
        IFQLProvider FQLProvider { get; }
        ISaw Saw { get; }
    }
}
