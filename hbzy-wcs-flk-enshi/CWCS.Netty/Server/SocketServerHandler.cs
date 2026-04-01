using CWCS.Netty.Common;
using DotNetty.Buffers;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CWCS.Netty.Server
{
    /// <summary>
    /// Author:zpw
    /// Description:socket server handler,you can define it by yourself
    /// </summary>
    public class SocketServerHandler : ChannelHandlerAdapter
    {
        public NotifyMessage _notifyMessage;

        //handler重复使用时，必须为true
        public override bool IsSharable => true;

        private List<IChannelHandlerContext> _contextList = new List<IChannelHandlerContext>();

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            base.HandlerAdded(context);
        }

        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            base.ChannelRegistered(context);
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            
            base.ChannelActive(context);
            AddContext(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
           
        }

        public void AddContext(IChannelHandlerContext context)
        {
            _contextList.Add(context);
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
            _contextList?.Remove(context);
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

        public void Write(string ip,string port,string message)
        {
            byte[] messageBytes = Encoding.Unicode.GetBytes(message);
            IByteBuffer initialMessage = Unpooled.Buffer(messageBytes.Length);
            initialMessage.WriteBytes(messageBytes);
            var item = _contextList;
        }

       
       


    }


}

