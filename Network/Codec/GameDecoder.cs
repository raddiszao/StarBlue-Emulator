using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using StarBlue.Communication.Packets.Incoming;
using StarBlue.Core;
using System;
using System.Collections.Generic;

namespace StarBlue.Network.Codec
{
    public class GameDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            try
            {
                if (input.ReadableBytes < 6) {
                    return;
                }

                input.MarkReaderIndex();
                int length = input.ReadInt();

                if (!(input.ReadableBytes >= length)) {
                    input.ResetReaderIndex();
                    return;
                }

                if (length < 0)
                {
                    return;
                }

                output.Add(new MessageEvent(input.ReadBytes(length)));
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Error in decode message");
            }
        }
    }
}