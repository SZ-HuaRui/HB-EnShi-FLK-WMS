using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CWCS.Netty.Server
{
    /// <summary>
    /// Author:zpw
    /// Description:socket server base on dotnetty
    public class SocketServer
    {
        private string Ip { get; set; }
        private string Port { get; set; }

        private ServerBootstrap _bootstrap;

        private IChannel _channel;

        private SocketServerHandler _socketServerHandler;
        public SocketServer(SocketServerHandler handler,string ip,string port)
        {
            Ip = ip;
            Port = port;
            _socketServerHandler = handler;
        }

        public async Task StartServer()
        {
            // 主工作线程组，设置为1个线程
            var bossGroup = new MultithreadEventLoopGroup(1);
            // 工作线程组，默认为内核数*2的线程数
            var workerGroup = new MultithreadEventLoopGroup();
            try
            {
                //声明一个服务端Bootstrap，每个Netty服务端程序，都由ServerBootstrap控制，
                //通过链式的方式组装需要的参数
                _bootstrap = new ServerBootstrap();
                _bootstrap
                    .Group(bossGroup, workerGroup) // 设置主和工作线程组
                    .Channel<TcpServerSocketChannel>() // 设置通道模式为TcpSocket
                    .Option(ChannelOption.SoBacklog, 100) // 设置网络IO参数等，这里可以设置很多参数，当然你对网络调优和参数设置非常了解的话，你可以设置，或者就用默认参数吧
                    .Option(ChannelOption.SoKeepalive, true)//保持连接
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                            //工作线程连接器 是设置了一个管道，服务端主线程所有接收到的信息都会通过这个管道一层层往下传输
                            //同时所有出栈的消息 也要这个管道的所有处理器进行一步步处理
                            IChannelPipeline pipeline = channel.Pipeline;

                            //业务handler ，这里是实际处理业务的Handler
                            pipeline.AddLast(_socketServerHandler);
                    }));

                // bootstrap绑定到指定端口的行为 就是服务端启动服务，同样的Serverbootstrap可以bind到多个端口
                _channel = await _bootstrap.BindAsync(IPAddress.Parse(Ip), Convert.ToInt32(Port));

            }
            catch (Exception)
            {
                await _channel?.CloseAsync();
                //释放工作组线程
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }

        }

    }
}
