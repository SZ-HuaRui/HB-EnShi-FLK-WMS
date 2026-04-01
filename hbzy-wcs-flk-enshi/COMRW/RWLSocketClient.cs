using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using CWCS.Common.Log;

namespace COMRW
{
    public delegate void NotifyCommandHander(string Command, object state);

    public class RWLSocketClient
    {
        public event NotifyCommandHander Notify;
        private int bytelength = 4096;

        private TcpClient cliSocket;
        private Thread ComThread;
        private bool g_brun;
        private readonly object qLock = new object();
        private int TimeOut = 1000;

        public RWLSocketClient(int length)
        {
            g_brun = false;
            bytelength = length;
        }

        public RWLSocketClient()
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
                    cliSocket.Client.Shutdown(SocketShutdown.Both);
                }
                cliSocket.Client.Close();
            }
            GC.Collect();
        }

        public bool Connected
        {
            get
            {
                if (cliSocket != null) //注意此值得含义
                    return cliSocket.Connected;
                else
                    return false;
            }
        }

        public bool Connect(string strServer, int nPort)
        {
            try
            {
                if (System.String.IsNullOrEmpty(strServer) || nPort == 0)
                { return false; }
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(strServer), nPort);
                cliSocket = TimeOutSocket.Connect(iep, TimeOut);
                if (cliSocket != null)
                {
                    RaiseNotifyEvent("Connect", " 连接成功！");
                    g_brun = true;
                    ComThread = new Thread(new ThreadStart(Recv));
                    ComThread.Start();
                    return true;
                }
                else
                {
                    RaiseNotifyEvent("Break", " 连接失败！");
                    return false;
                }
            }
            catch (SocketException ex)
            {
                RaiseNotifyEvent("Break", " 连接失败,异常:" + ex.Message);
                return false;
            }
        }

        public bool CheckConnect(string strServer, int nPort)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(strServer), nPort);
            cliSocket = TimeOutSocket.Connect(iep, TimeOut);
            if (cliSocket != null)
                return true;
            else
                return false;
        }

        public bool Send(byte[] bytData)
        {
            try
            {
                lock (qLock)
                {
                    if (cliSocket.Client.Connected)
                    {
                        cliSocket.Client.Send(bytData);
                        return true;
                    }
                    else
                    {
                        Close();
                        RaiseNotifyEvent("Break", "SendData Fail!" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mi:ss"));
                        return false;
                    }
                }
            }
            catch (SocketException ex)
            {
                Close();
                RaiseNotifyEvent("Break", ex.Message);
                return false;
            }
        }

        public void Recv()
        {
            byte[] bytBuffer = new byte[bytelength];// 报文长度
            while (g_brun)
            {
                try
                {
                    if (cliSocket != null)
                    {
                        if (cliSocket.Client.Connected)
                        {
                            for (int i = 0; i < bytBuffer.Length; i++)
                            {
                                bytBuffer[i] = 0;
                            }
                            int nLen = cliSocket.Client.Receive(bytBuffer);
                            if (nLen > 0)
                            {
                                //str = System.Text.Encoding.ASCII.GetString(bytBuffer);
                                RaiseNotifyEvent("RecvData", bytBuffer);
                               
                            }
                            else
                            {
                                //连接断开
                                Close();
                                RaiseNotifyEvent("Break", " 接收到的报文数据长度为0！");
                                break;
                            }
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Close();
                    RaiseNotifyEvent("Break", " 连接中断！" + ex.Message);
                }
            }
        }

        public void RaiseNotifyEvent(string Command, object oState)
        {
            if (this.Notify != null)
            {
                Notify(Command, oState);
            }
        }
    }
}
