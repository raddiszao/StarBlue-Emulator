using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using log4net;
using StarBlue.Communication.Packets.Incoming;
using StarBlue.Communication.Packets.Outgoing.Misc;
using StarBlue.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace StarBlue.Network
{
    public class NetworkChannelHandler : SimpleChannelInboundHandler<MessageEvent>
    {
        public override void ChannelActive(IChannelHandlerContext context)
        {
            StarBlueServer.GetGame().GetClientManager().CreateAndStartClient(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            string IpAddress = ((IPEndPoint)context.Channel.RemoteAddress).Address.MapToIPv4().ToString();
            NetworkBootstrap.IncrementCounterForIp(IpAddress, (NetworkBootstrap.IpConnectionCount(IpAddress) - 1));
            if (StarBlueServer.GetGame().GetClientManager().TryGetClient(context.Channel.Id, out var client))
                StarBlueServer.GetGame().GetClientManager().DisposeConnection(context.Channel.Id);
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            if (exception is IOException || exception is SocketException) return;

            Logging.HandleException(exception, "Error in channel handler");
        }

        protected override void ChannelRead0(IChannelHandlerContext context, MessageEvent msg)
        {
            try
            {
                if (StarBlueServer.GetGame().GetClientManager().TryGetClient(context.Channel.Id, out var client))
                    StarBlueServer.GetGame().GetPacketManager().TryExecutePacket(client, msg);
            } catch (Exception e)
            {
                Logging.HandleException(e, "Error while receiving message");
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (NetworkBootstrap.IdleTimerEnabled)
            {
                if (evt is IdleStateEvent)
                {
                    IdleStateEvent e = (IdleStateEvent)evt;
                    if (e.State == IdleState.ReaderIdle)
                    {
                        context.DisconnectAsync();
                        context.CloseAsync();
                    }
                    else if (e.State == IdleState.WriterIdle)
                    {
                        context.WriteAndFlushAsync(new PingMessageComposer());
                    }
                }
            }

            if (evt.GetType().IsInstanceOfType(ChannelInputShutdownEvent.Instance))
            {
                context.DisconnectAsync();
                context.CloseAsync();
            }
        }

        public override bool IsSharable => true;

    }
}