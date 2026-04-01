using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Core.Library.PLCManager
{
    public interface IOPCServer
    {
        //bool InitServer(string ip, int slot, int port = 102);
        bool Connect();
        bool Write(int DBNumber, int start, int len, byte[] msg);
        bool Read(int DBNumber, int start, int len, ref byte[] msg);
        void DisConnect();
    }
}
