using CWCS.Common.Log;
using CWCS.Common.Uitl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWCS.NetWork
{
    /// <summary>
    /// Author:zpw
    /// Description:Client,Send and Receive
    /// </summary>
    public class SocketClientBoth : NetFixedCommand
    {
        public delegate void DelegateNotifyStringEvent(SocketEvent socketEvent, Socket socket, string message);

        public delegate void DelegateNotifyByteEvent(SocketEvent socketEvent, Socket socket, byte[] receiveByte);
        public string IP { get; set; }
        public string Port { get; set; }
        public Socket CoreSocket { get; set; }
        public bool IsConnected { get; set; } = false;

        public Timer CheckConnectionTimer = null;
        public bool IsCheckTimerStarted { get; set; } = false;

        public object LockObject = new object();
        public Thread ReceiveThread { get; set; }

        public event DelegateNotifyStringEvent NotifyStringEvent;

        public event DelegateNotifyByteEvent NotifyByteEvent;


        public SocketClientBoth(string ip, string port)
        {
            IP = ip;
            Port = port;
        }

        public bool Connect()
        {
            lock (LockObject)
            {
                try
                {
                    if (!IsConnected)
                    {
                        if (!IsCheckTimerStarted)
                        {
                            CheckConnectionTimer = new Timer((P) => CheckConnection(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
                            IsCheckTimerStarted = true;
                        }
                        IPAddress iPAddress = IPAddress.Parse(IP);
                        CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        CoreSocket.Connect(new IPEndPoint(iPAddress, Convert.ToInt32(Port)));
                        ReceiveThread = new Thread(ReceiveMessage);
                        ReceiveThread.Start(CoreSocket);
                        IsConnected = true;
                        NotifyStringEvent?.Invoke(SocketEvent.Connected, CoreSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:连接{IP}:{Port}成功！");
                        return IsConnected;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    NotifyStringEvent?.Invoke(SocketEvent.Break, CoreSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:连接{IP}:{Port}失败！" + ex.Message);
                    return IsConnected;
                }
            }

        }

        public void ReceiveMessage(object socket)
        {
            Socket receiveSocket = socket as Socket;
            while (true)
            {
                try
                {
                    if (IsConnected && receiveSocket != null)
                    {
                        byte[] byteArray = new byte[1024];
                        int receiveLength = receiveSocket.Receive(byteArray);
                        if (receiveLength < 1)
                        {
                            IsConnected = false;
                            NotifyStringEvent?.Invoke(SocketEvent.Break, receiveSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:接收数据{IP}：{Port}数据失败！客户端主动断开，开始重连！");
                            return;
                        }
                        NotifyByteEvent?.Invoke(SocketEvent.Receive, receiveSocket, DataTransformUtil.ByteArrayTrimNull(byteArray));
                    }
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    NotifyStringEvent?.Invoke(SocketEvent.Break, receiveSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:接收数据{IP}：{Port}数据失败！客户端主动断开，开始重连！{ex.Message}");
                    return;
                }

            }
        }


        public void Close()
        {
            try
            {
                if (CoreSocket != null && CoreSocket.Connected)
                {
                    CoreSocket.Shutdown(SocketShutdown.Both);
                    CoreSocket.Close();
                }
                IsConnected = false;
            }
            catch
            {
                IsConnected = false;
            }
           
        }

        public bool Send(byte[] sendByte)
        {
            if (!IsConnected || CoreSocket == null)
            {
                NotifyStringEvent?.Invoke(SocketEvent.Break, CoreSocket, $"\n{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:发送消息到{IP}：{Port}失败！因为未连接到服务端！");
                return false;
            }
            try
            {
                CoreSocket.Send(sendByte);
                return true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
                NotifyStringEvent?.Invoke(SocketEvent.Break, CoreSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:发送消息到{IP}：{Port}失败！{ex.Message}");
                return false;
            }
        }

        public bool SendHeart()
        {
            if (!IsConnected || CoreSocket == null)
            {
                NotifyStringEvent?.Invoke(SocketEvent.Break, CoreSocket, $"\n{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:发送心跳到{IP}：{Port}失败！因为未连接到服务端！");
                return false;
            }
            try
            {
                byte[] packet = PackHeartBeat(InSortingHeartBeat);
                CoreSocket.Send(packet);
               // WriteReportLog("WCS发送给分拣心跳", packet);
                IsConnected = true;
                return IsConnected;
            }
            catch (Exception ex)
            {
                NotifyStringEvent?.Invoke(SocketEvent.Break, CoreSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:发送心跳到{IP}：{Port}失败！{ex.Message}");
                IsConnected = false;
                return IsConnected;
            }
        }

        public void WriteReportLog(string message, byte[] bytes)
        {
            StringBuilder writeStr = new StringBuilder();
            if (bytes.Length > 0)
            {
                foreach (byte b in bytes)
                {
                    writeStr.Append(b.ToString("X2"));
                }
                LogUtil.Write("入库分拣", "心跳报文", "心跳报文",$"{message}。{writeStr.ToString()}");
            }
        }




        public void CheckConnection()
        {
            try
            {
                if (!IsConnected)
                {
                    Close();
                    Connect();
                }
                else
                {
                    SendHeart();
                }
            }
            catch (Exception ex)
            {
                NotifyStringEvent?.Invoke(SocketEvent.Break, CoreSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:心跳检测进程出现异常！{ex.Message}");
                IsConnected = false;
            }

        }


    }
}
