using CWCS.Netty.Common;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CWCS.Netty.Client
{

    /// <summary>
    /// Author:zpw
    /// Description:socket client handler,you can define it by yourself
    /// </summary>
    public class SocketClientHandler : ChannelHandlerAdapter
    {

        public NotifyMessage _notifyMessage;

        public SocketClient _socketClient;

        //handler重复使用时，必须为true
        public override bool IsSharable => true;

        public IChannelHandlerContext _context;

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            base.HandlerAdded(context);
            _context = context;
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            base.ChannelRegistered(context);
        }


        public override void ChannelActive(IChannelHandlerContext context)
        {
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            StartReConnect();
            base.ChannelInactive(context);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer buffer)
            {
                string data = buffer.ToString(Encoding.GetEncoding("Unicode"));
                string recieveText = "recieve:" + context.Channel.RemoteAddress.ToString() + "-> " +
                                context.Channel.LocalAddress.ToString() + "\r\n" +
                                (string.IsNullOrEmpty(data) ? "" : data.Trim(new char[] { '\r', '\n' }));
                _notifyMessage?.Invoke(SocketEvent.Receive, recieveText);

            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }


        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            StartReConnect();
            base.HandlerRemoved(context);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent)
            {
                var eventState = evt as IdleStateEvent;
                if (eventState != null)
                {
                    switch (eventState.State)
                    {
                        //接收服务端数据
                        case IdleState.ReaderIdle:
                            break;
                        //向服务端发送数据
                        case IdleState.WriterIdle:


                            break;
                        case IdleState.AllIdle:
                            break;
                    }
                }
            }
        }

        public void Write(string message)
        {
            byte[] messageBytes = Encoding.Unicode.GetBytes(message);
            IByteBuffer initialMessage = Unpooled.Buffer(messageBytes.Length);
            initialMessage.WriteBytes(messageBytes);
            _context.WriteAndFlushAsync(initialMessage);

        }

        public void StartReConnect()
        {
            _socketClient?.manualResetEvent.Set();
        }


        public void StopReConnect() => _socketClient?.manualResetEvent.Reset();



}

    
}

