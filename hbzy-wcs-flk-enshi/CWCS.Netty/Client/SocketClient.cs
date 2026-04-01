using CWCS.Netty.Common;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWCS.Netty.Client
{
    /// <summary>
    /// Author:zpw
    /// Description:socket client base on dotnetty
    /// </summary>
    public class SocketClient
    {
        public string Ip { get; set; }
        public string Port { get; set; }

        public Bootstrap _bootstrap;

        public IChannel _channel;

        private SocketClientHandler _socketClientHandler;

        public ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        public SocketClient(SocketClientHandler handler,string ip,string port)
        {
            Ip = ip;
            Port = port;
            _socketClientHandler = handler;
            _socketClientHandler._socketClient = this;
            Init();
            addConnectionTask();
        }

        public void Init()
        {
                var group = new MultithreadEventLoopGroup();
                _bootstrap = new Bootstrap();
                _bootstrap.Group(group)
                       .Channel<TcpSocketChannel>()
                       .Option(ChannelOption.TcpNodelay, true)
                       .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                       {
                           IChannelPipeline pipeline = channel.Pipeline;
                           pipeline.AddLast(_socketClientHandler);
                           //1.读间隔；2.写间隔；3.读写间隔
                           pipeline.AddLast("idleStateHandle", new IdleStateHandler(10, 0, 0));
                       }));
               
        }
        public async Task Connect()
        {
            try
            {
                if (_channel != null)
                {
                    await _channel.CloseAsync();
                }
                _channel = await _bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse(Ip), Convert.ToInt32(Port)));
                manualResetEvent.Reset();
                _socketClientHandler._notifyMessage?.Invoke(SocketEvent.Connected, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:连接{Ip}:{Port}成功！");
            }
            catch (Exception ex)
            {
                _socketClientHandler._notifyMessage?.Invoke(SocketEvent.Break, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:{ex.Message}");
                manualResetEvent.Set();
            }
        }

        public void addConnectionTask()
        {
             Task.Factory.StartNew(async() =>
            {
                while (true)
                {
                    manualResetEvent.WaitOne();
                    _socketClientHandler._notifyMessage?.Invoke(SocketEvent.Break, $"\n{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}:客户端主动断开!开始重连！");
                    await Connect();
                    await Task.Delay(2000);
                }
            });
        }
       
    }
}
