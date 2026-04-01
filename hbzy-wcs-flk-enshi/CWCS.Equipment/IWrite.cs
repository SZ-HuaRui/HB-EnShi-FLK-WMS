using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment
{
    public interface IWrite
    {
        bool WriteString(string message);
        bool WriteBytes(byte[] bytes);
    }
}
