using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment
{
    public interface ISend
    {
        bool SendString(string message);
        bool SendBytes(byte[] bytes);
    }
}
