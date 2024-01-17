using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDSL;

internal class SDSLInvalidKeyName : Exception 
{
    public SDSLInvalidKeyName() {}
    public SDSLInvalidKeyName(string message) : base(message) {} 
}
