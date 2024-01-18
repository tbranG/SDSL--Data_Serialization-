using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDSL;

internal class SDSLInvalidFileExtension: Exception
{
    public SDSLInvalidFileExtension() {}
    public SDSLInvalidFileExtension(string message) : base(message) {}
}
