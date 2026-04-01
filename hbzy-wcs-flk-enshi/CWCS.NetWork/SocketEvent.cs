using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.NetWork
{
    public enum SocketEvent : int
    {
        Connected = 0,
        Send = 1,
        Receive = 2,
        Break = 3
    }
}
