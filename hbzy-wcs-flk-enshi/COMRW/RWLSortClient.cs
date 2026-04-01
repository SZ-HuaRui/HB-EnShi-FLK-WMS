using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;

namespace COMRW
{
    //public delegate void NotifyCommandHander(string Command, object state);

    public class RWLSortClient
    {
        public event NotifyCommandHander Notify;
        private int bytelength = 256;

        Thread thread;
        Thread thread2;
        private bool bRound = true;
        private static Socket serSocket;
        private static Socket accsock;
        private int _nIpPort;
        private string _strIpAddress;
        private EndPoint myServer;
        private Socket cliSocket;
        private Thread ComThread;
        private bool g_brun;
        private string bufferstring;
        private string[] subbuffer;
        private readonly object qLock = new object();
        private int TimeOut = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TimeOut"].ToString());
        private string wcsip = System.Configuration.ConfigurationManager.AppSettings["WCSIP"].ToString();
        private int wcsport = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WCSPort"].ToString());
        StringBuilder strRecvMessage = new StringBuilder();
        public RWLSortClient(int length)
        {
            g_brun = false;
            bytelength = length;
        }

        public RWLSortClient()
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
            //GC.Collect();
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
        public void CloseSwisslog()
        {
            g_brun = false;

            if (thread != null)
            {
                if (thread.ThreadState == ThreadState.Running)
                {
                    thread.Abort();
                }
                Thread.Sleep(50);
            }

            if (serSocket != null)
            {
                if (serSocket.Connected)
                {
                    serSocket.Shutdown(SocketShutdown.Both);
                }
                serSocket.Close();
            }
            //GC.Collect();
        }
        public bool ConnectSwisslog()
        {
            try
            {
                thread = new Thread(new ThreadStart(accp)); //生成监听线程；
                thread.IsBackground = false;//主线程关闭则辅线程自动关闭
                thread.Start();
                return true;
            }
            catch (Exception ex)
            {
                RaiseNotifyEvent("Break", "无法监听:" + ex.Message);
                return false;
            }
        }
        private void accp()
        {
            myServer = new IPEndPoint(IPAddress.Parse(wcsip), wcsport);
            serSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serSocket.Bind(myServer);
                RaiseNotifyEvent("Listen", "主机:" + wcsip + " 端口:" + wcsport + " 开始监听！");
                serSocket.Listen(500);
                while (true)
                {
                    accsock = serSocket.Accept();
                    if (accsock.Connected)
                    {
                        Byte[] recBuff = new byte[1024];
                        RaiseNotifyEvent("Listen", "主机:" + wcsip + " 端口:" + wcsport + " 与客户建立连接！");
                        //int nLen=accsock.Receive(recBuff);
                        //if(nLen>0)
                        //    RaiseNotifyEvent("RecvData", recBuff);
                        thread2 = new Thread(new ThreadStart(round));//如果有客户请求，生成与客户通信的线程。
                        thread2.Start();
                        thread2.IsBackground = false;//主线程关闭则辅线程自动关闭
                        bRound = true;
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseNotifyEvent("Listen", "主机:" + wcsip + " 端口:" + wcsport + "监听异常:" + ex.Message);
            }
        }

        private void round()
        {
            while (bRound)
            {
                try
                {
                    if (accsock != null && accsock.Connected)
                    {
                        Byte[] recBuff = new byte[1024];
                        NetworkStream netStream = new NetworkStream(accsock);

                        netStream.Read(recBuff, 0, recBuff.Length);//读取客户发送来的信息。
                        bufferstring = System.Text.ASCIIEncoding.ASCII.GetString(recBuff);
                        subbuffer = bufferstring.Split('>');
                        for (int i = 0; i < subbuffer.Length; i++)
                        {
                            int j = subbuffer[i].LastIndexOf("<");
                            if (j >= 0)
                                RaiseNotifyEvent("RecvData", Encoding.Default.GetBytes(subbuffer[i].Substring(j, (subbuffer[i].Length - j))));
                        }
                        // RaiseNotifyEvent("RecvData", recBuff);
                        netStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    RaiseNotifyEvent("", "主机:" + wcsip + " 端口:" + wcsport.ToString() + "获取数据异常:" + ex.Message);
                    bRound = false;
                }
            }
        }
        public void SendMessage(byte[] sendbyte)
        {
            try
            {
                if (accsock != null && accsock.Connected)
                {
                    NetworkStream netStream = new NetworkStream(accsock);//生成NetworkStream实例，用于发送基础数据流。
                    netStream.Write(sendbyte, 0, sendbyte.Length);
                    netStream.Close();
                }
            }
            catch (Exception ex)
            {
                RaiseNotifyEvent("SendData", "主机:" + wcsip + " 端口:" + wcsport.ToString() + "发送数据异常:" + ex.Message);
            }
        }
        public bool CheckConnect(string strServer, int nPort)
        {
            return cliSocket.Connected;
            //IPEndPoint iep = new IPEndPoint(IPAddress.Parse(strServer), nPort);

            //cliSocket = TimeOutSocket.Connect(iep, TimeOut);
            //if (cliSocket != null)
            //    return true;
            //else
            //    return false;
        }
        public bool Connect(string strServer, int nPort)
        {
            try
            {
                if (System.String.IsNullOrEmpty(strServer) || nPort == 0)
                { return false; }

                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(strServer), nPort);
                cliSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (cliSocket.Connected)
                {
                    Close();
                }
                cliSocket.Connect(iep);
                //cliSocket.Bind(new IPEndPoint(IPAddress.Parse(wcsip), wcsport));
                if (cliSocket != null)
                {
                    RaiseNotifyEvent("Connect", " Socket 连接成功！");
                    g_brun = true;
                    ComThread = new Thread(new ThreadStart(Recv));//处理收到的数据
                    ComThread.Start();
                    return true;
                }
                else
                {
                    RaiseNotifyEvent("Break", " Socket 连接失败！");
                    return false;
                }
            }
            catch (SocketException ex)
            {
                RaiseNotifyEvent("Break", " 连接失败,异常:" + ex.Message);
                return false;
            }
        }

        public bool Send(byte[] bytData)
        {
            try
            {
                lock (qLock)
                {
                    if (cliSocket.Connected)
                    {
                        cliSocket.Send(bytData);
                        string msg = System.Text.Encoding.ASCII.GetString((byte[])bytData);
                        RaiseNotifyEvent("Send", "SendData Success!" + msg);
                        return true;
                    }
                    else
                    {
                        Close();
                        RaiseNotifyEvent("Break", "SendData Fail!");
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
                        if (cliSocket.Connected)
                        {
                            for (int i = 0; i < bytBuffer.Length; i++)
                            {
                                bytBuffer[i] = 0;
                            }
                            int nLen = cliSocket.Receive(bytBuffer);
                            if (nLen > 0)
                            {
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

        private void RaiseNotifyEvent(string Command, object oState)
        {
            if (this.Notify != null)
            {
                Notify(Command, oState);
            }
        }
    }
}
