using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace CWCS.Core.Library
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Collections;


    /// <summary>
    /// 建立Socket异步连接
    /// </summary>
    public class TimeOutSocket
    {
        private bool IsConnectionSuccessful = false;
        private Exception socketexception;
        private ManualResetEvent TimeoutObject = new ManualResetEvent(false);

        public Socket Connect(IPEndPoint remoteEndPoint, int timeoutMSec=3000)
        {
            TimeoutObject.Reset();
            socketexception = null;

            string serverip = Convert.ToString(remoteEndPoint.Address);
            int serverport = remoteEndPoint.Port;

            Socket tcpclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint endPoint = (EndPoint)remoteEndPoint;
            tcpclient.BeginConnect(endPoint, new AsyncCallback(CallBackMethod), tcpclient);

            if (TimeoutObject.WaitOne(timeoutMSec, false))
            {
                if (IsConnectionSuccessful)
                {
                    return tcpclient;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                tcpclient.Close();
                return null;
            }
        }
        private void CallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                Socket Client = (Socket)asyncresult.AsyncState;
                if (Client != null)
                {
                    Client.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
                socketexception = ex;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }
    }

}
