using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Collections.Generic;
using System.Text;

namespace StarBlue.Network.Codec
{
    public class GamePolicyDecoder : ByteToMessageDecoder
    {
        private string POLICY = "<?xml version=\"1.0\"?>\r\n"
                             + "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n"
                             + "<cross-domain-policy>\r\n"
                             + "<allow-access-from domain=\"*\" to-ports=\"*\" />\r\n"
                             + "</cross-domain-policy>\0)";

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            message.MarkReaderIndex();
            if (message.ReadableBytes < 1) return;

            byte delimiter = message.ReadByte();

            message.ResetReaderIndex();
            if (delimiter == 0x3C)
            {
                context.WriteAndFlushAsync(Unpooled.CopiedBuffer(Encoding.Default.GetBytes(POLICY)));
                return;
            }

            context.Channel.Pipeline.Remove(this);
        }
    }
}