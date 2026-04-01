using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment
{
    public interface IRead
    {
        string ReadString();
        byte[] ReadBytes();
    }
}
