using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDSL.Exceptions;

internal class SDSLInvalidKeyName : Exception
{
    public SDSLInvalidKeyName() { }
    public SDSLInvalidKeyName(string message) : base(message) { }
}
