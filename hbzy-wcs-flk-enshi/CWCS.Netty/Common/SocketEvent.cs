using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Netty.Common
{
    public enum SocketEvent:int
    {
        Connected = 0,
        Break = 1,
        Receive = 2,
        Send = 3,
        Heart = 4
    }
}
