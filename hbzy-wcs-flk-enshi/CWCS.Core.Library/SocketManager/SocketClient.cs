using CWCS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Messaging;


namespace CWCS.Core.Library.SocketManager
{
    public delegate void NotifyCommandHander(NotifyCommand Command, object state);
    public class SocketClient
    {
        public event NotifyCommandHander Notify;
        private int bytelength = 1024;

        private Socket cliSocket;
        private TcpClient tcpSocket;
        private Thread ComThread;
        private bool g_brun;
        private readonly object qLock = new object();
        private int TimeOut = 5000;//int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeOut"].ToString());
        StringBuilder strRecvMessage = new StringBuilder();
        public SocketClient(int length)
        {
            g_brun = false;
            bytelength = length;
            
        }

        public SocketClient()
        {
            g_brun = false;
        }

        public void Close()
        {
            g_brun = false;

            if (ComThread != null)
            {
                if (ComThread.ThreadState == ThreadState.Running)
                {
                    ComThread.Abort();
                }
                Thread.Sleep(50);
            }

            if (cliSocket != null)
            {
                if (Connected)
                {
                    cliSocket.Shutdown(SocketShutdown.Both);
                }
                cliSocket.Close();

            }
            GC.Collect();
        }

        public bool Connected
        {
            get
            {
                if (cliSocket != null)
                    return cliSocket.Connected;
                else
                    return false;
            }
        }

        public bool Connect(string strServer, int nPort)
        {
            try
            {
                if (string.IsNullOrEmpty(strServer) || nPort == 0)
                { return false; }
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(strServer), nPort);
                
                cliSocket = (new TimeOutSocket()).Connect(iep, TimeOut);

                if (cliSocket != null)
                {
                    g_brun = true;
                    if (nPort != 2000)
                    {
                        ComThread = new Thread(new ThreadStart(Recv));//处理收到的数据
                        ComThread.Start();
                        RaiseNotifyEvent(NotifyCommand.Connect, string.Format("WCS与IP:{0},Port:{1},接收通道已开启！", strServer,nPort));
                    }
                 
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SocketException ex)
            {
                RaiseNotifyEvent(NotifyCommand.Break, string.Format(" WCS与IP:{0},Port:{1},连接异常:{2}",strServer,nPort,ex.Message));
                return false;
            }
        }

        public bool CheckConnect(string strServer, int nPort)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(strServer), nPort);
            cliSocket = (new TimeOutSocket()).Connect(iep, TimeOut);
            if (cliSocket != null)
                return true;
            else
                return false;
        }
        //发送数据
        public bool Send(byte[] bytData)
        {
            try
            {
                lock (qLock)
                {
                    if (cliSocket!=null && cliSocket.Connected)
                    {
                        cliSocket.Send(bytData);
                        return true;
                    }
                    else
                    {
                        Close();
                        //RaiseNotifyEvent(NotifyCommand.Break, string.Format("WCS下发数据失败，IP:{0},Port:{1}", 1,2));
                        return false;
                    }
                }
            }
            catch (SocketException ex)
            {
                Close();
                RaiseNotifyEvent(NotifyCommand.Break, ex.Message);
                return false;
            }
        }

        //异步接收报文数据
        public static byte[] ReceiveData(Socket socket)
        {
            ReceiveObject state = new ReceiveObject() { Client = socket };
            socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), state);
            state.ReceiveDone.WaitOne(1000 * 10); // 10秒超时
            byte[] result = state.Stream.ToArray();
            return result;
        }

        /// <summary>

        /// read回调

        /// </summary>

        /// <param name="ar"></param>

        private static void ReadCallback(IAsyncResult ar)
        {
            ReceiveObject state = (ReceiveObject)ar.AsyncState;
            int bytesRead = state.Client.EndReceive(ar);
            if (bytesRead > 0)
            {
                try
                {
                    state.Stream.Write(state.Buffer, 0, bytesRead);
                    state.Client.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, new AsyncCallback(ReadCallback), state);
                }
                catch (Exception ex)
                {
                    state.ReceiveDone.Set();
                }
            }
            else
            {
                state.ReceiveDone.Set();
            }
        }

        //接收数据
        public void Recv()
        {
            byte[] bytBuffer = new byte[bytelength];// 报文长度
            while (g_brun)
            {
                try
                {
                    if (cliSocket != null)
                    {
                        if (cliSocket.Connected)
                        {
                            for (int i = 0; i < bytBuffer.Length; i++)
                            {
                                bytBuffer[i] = 0;
                            }

                            int nLen = cliSocket.Receive(bytBuffer);
                            if (nLen > 0)
                            {
                                RaiseNotifyEvent(NotifyCommand.RecvData, bytBuffer);
                            }
                            else
                            {
                                Close();
                                //连接断开
                                RaiseNotifyEvent(NotifyCommand.Break, null);
                                break;
                            }
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Close();
                    RaiseNotifyEvent(NotifyCommand.Break, "连接中断！" + ex.Message);
                }
            }
        }

        private void RaiseNotifyEvent(NotifyCommand Command, object oState)
        {
            if (this.Notify != null)
            {
                Notify?.Invoke(Command, oState);
            }
        }
    }

   
}
public class ReceiveObject
{
    public Socket Client;
    public byte[] Buffer = new byte[10];
    public System.IO.MemoryStream Stream = new System.IO.MemoryStream();
    public System.Threading.ManualResetEvent ReceiveDone = new System.Threading.ManualResetEvent(false);
}
