using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Equipment.Led.Yb
{
    public abstract class YbLedBase:LedBase
    {
        public YbLedBase(string ip, string port) : base(ip, port)
        {

        }
    }
}
