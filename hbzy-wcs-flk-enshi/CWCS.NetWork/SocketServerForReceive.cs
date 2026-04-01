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
    /// Description:Server,Send and Receive
    /// </summary>
    public class SocketServerForReceive : NetFixedCommand
    {
        public delegate void DelegateNotifyStringEvent(SocketEvent socketEvent,Socket socket, string message);

        public delegate void DelegageNotifyByteEvent(SocketEvent socketEvent, Socket socket, byte[] receiveByte);
        public string IP { get; set; }
        public string Port { get; set; }
        public Socket ServerSocket { get; set; }
        public Thread ListenThread { get; set; }
        public Thread ReceiveThread { get; set; }

        public event DelegateNotifyStringEvent StringEvent;

        public event DelegageNotifyByteEvent ByteEvent;
        public SocketServerForReceive(string ip, string port)
        {
            IP = ip;
            Port = port;
        }

        public bool StartServer()
        {
            try
            {
                IPAddress iPAddress = IPAddress.Parse(IP);
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.Bind(new IPEndPoint(iPAddress, Convert.ToInt32(Port)));
                ServerSocket.Listen(10);
                ListenThread = new Thread(ListenClient);
                ListenThread.IsBackground = true;
                ListenThread.Start();
                return true;
            }
            catch (Exception ex)
            {
                StringEvent?.Invoke(SocketEvent.Break, ServerSocket, $@"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:服务端{IP}:{Port}。启动异常！{ex.Message}");
                return false;
            }

        }

        public void ListenClient()
        {
            while (true)
            {
                Socket newSocket = null;
                try
                {
                    newSocket = ServerSocket.Accept();
                    ReceiveThread = new Thread(ReceiveMessage);
                    ReceiveThread.IsBackground = true;
                    ReceiveThread.Start(newSocket);
                }
                catch (Exception ex)
                {
                    StringEvent?.Invoke(SocketEvent.Break, newSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:服务端{IP}:{Port}。监听服务异常！{ex.Message}");
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
                    if (receiveSocket.Connected)
                    {
                        byte[] byteArray = new byte[1024];
                        int receiveLength = receiveSocket.Receive(byteArray);
                        if (receiveLength < 1)
                        {
                            StringEvent?.Invoke(SocketEvent.Break, receiveSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:服务端{IP}:{Port}。接收客户端数据异常。接收长度为0");
                            if (receiveSocket != null && receiveSocket.Connected)
                            {
                                receiveSocket?.Shutdown(SocketShutdown.Both);
                                receiveSocket.Close();
                            }
                            return;
                        }
                        ByteEvent?.Invoke(SocketEvent.Receive,receiveSocket,DataTransformUtil.ByteArrayTrimNull(byteArray));
                    }
                }
                catch (Exception ex)
                {
                    StringEvent?.Invoke(SocketEvent.Break, receiveSocket, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:服务端{IP}:{Port}。接收客户端数据出现异常！{ex.Message}");
                    if (receiveSocket != null && receiveSocket.Connected)
                    {
                        receiveSocket?.Shutdown(SocketShutdown.Both);
                        receiveSocket.Close();
                    }
                    return;
                }

            }
        }

    }
}
