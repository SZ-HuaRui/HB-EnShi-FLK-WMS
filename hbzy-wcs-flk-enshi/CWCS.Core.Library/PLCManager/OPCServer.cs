using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CWCS.Core.Library.PLCManager
{
    /// <summary>
    /// OPC服务 
    /// </summary>
    public class OPCServer : IOPCServer
    {
        PlcTcpClient mPLC = null;

        public OPCServer(string ip, int slot, int port = 102)
        {
            mPLC = new PlcTcpClient(ip, port, slot);
        }

        public bool Connect()
        {
            int i = mPLC.ConnectPLC();
            if (i == 0)
                return true;
            return false;
        }

        public bool Write(int DBNumber, int start, int len, byte[] msg)
        {
            if (mPLC.IsConnected)
            {
                int i = mPLC.WriteManyBytes(PlcMemoryArea.DB, DBNumber, start, len, msg);
                if (i == 0)
                    return true;
            }
            return false;
        }

        public bool Read(int DBNumber, int start, int len, ref byte[] msg)
        {
            lock (this)
            {
                if (mPLC.IsConnected)
                {
                    int i = mPLC.ReadManyBytes(PlcMemoryArea.DB, DBNumber, start, len, msg);
                    if (i == 0)
                        return true;
                }
                return false;
            }
        }

        public void DisConnect()
        {
            try
            {
                mPLC.DisConnectPLC();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
