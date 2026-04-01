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
    /// Description:Client, Only Send
    /// </summary>
    public class SocketClientForSend : NetFixedCommand
    {
        public delegate void DelegateNotifyStringEvent(SocketEvent socketEvent, string message);
        public delegate void DelegateNotifyByteEvent(SocketEvent socketEvent, string message);
        public string IP { get; set; }
        public string Port { get; set; }
        public Socket CoreSocket { get; set; }
        public bool IsConnected { get; set; } = false;

        public event DelegateNotifyStringEvent NotifyEvent;

        public Timer CheckConnectionTimer = null;
        public bool IsCheckTimerStarted { get; set; } = false;

        public object LockObject = new object();


        public SocketClientForSend(string ip, string port)
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
                            CheckConnectionTimer = new Timer((P) => CheckConnection(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
                            IsCheckTimerStarted = true;
                        }
                        IPAddress iPAddress = IPAddress.Parse(IP);
                        CoreSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        CoreSocket.Connect(new IPEndPoint(iPAddress, Convert.ToInt32(Port)));
                        IsConnected = true;
                        NotifyEvent?.Invoke(SocketEvent.Connected, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:连接{IP}:{Port}成功！");
                        return IsConnected;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    NotifyEvent?.Invoke(SocketEvent.Break, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:连接{IP}:{Port}失败！" + ex.Message);
                    return IsConnected;
                }
            }

        }


        public void Close()
        {
            if (CoreSocket != null && CoreSocket.Connected)
            {
                CoreSocket?.Shutdown(SocketShutdown.Both);
                CoreSocket?.Close();
                IsConnected = false;
                NotifyEvent?.Invoke(SocketEvent.Break, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:关闭到{IP}:{Port}的连接！");
            }
        }

        public bool Send(byte[] sendByte)
        {
            if (!IsConnected || CoreSocket == null)
            {
                NotifyEvent?.Invoke(SocketEvent.Break, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:发送消息到{IP}:{Port}失败！因为未连接到服务端！");
                return false;
            }
            try
            {
                CoreSocket.Send(sendByte);
                return true;
            }
            catch (Exception ex)
            {
                NotifyEvent?.Invoke(SocketEvent.Break, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:发送消息到{IP}:{Port}失败！" + ex.Message);
                return false;
            }
        }


        public void CheckConnection()
        {
            if (!IsConnected)
            {
                Close();
                Connect();
            }
        }



    }
}
