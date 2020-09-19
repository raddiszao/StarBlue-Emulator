using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using StarBlue.Communication.Packets.Outgoing;
using StarBlue.Core;
using System;

namespace StarBlue.Network.Codec
{
    public class GameByteEncoder : MessageToByteEncoder<MessageComposer>
    {
        protected override void Encode(IChannelHandlerContext context, MessageComposer message, IByteBuffer buffer)
        {
            try
            {
                Composer composer = message.WriteMessage(buffer);

                if (!composer.IsFinalized())
                    composer.Content.SetInt(0, composer.Content.WriterIndex - 4);
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Error in encode message");
            }
        }
    }
}