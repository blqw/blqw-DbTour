using System;
using System.Collections.Generic;
using System.Text;

namespace blqw
{
    public interface IDbComponentFactory
    {
        IFQLProvider CreateFQLProvider();

        ISaw CreateSaw();
    }
}
