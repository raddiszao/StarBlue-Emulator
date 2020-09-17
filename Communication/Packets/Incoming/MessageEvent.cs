using DotNetty.Buffers;
using System.Text;

namespace StarBlue.Communication.Packets.Incoming
{
    public class MessageEvent
    {
        private IByteBuffer buffer;
        public short Id { get; }

        public MessageEvent(IByteBuffer buf)
        {
            this.buffer = (buf != null) && (buf.ReadableBytes > 0) ? buf : Unpooled.Empty;

            this.Id = buf.ReadShort();
        }

        public string PopString()
        {
            int length = buffer.ReadShort();
            IByteBuffer data = buffer.ReadBytes(length);
            return Encoding.Default.GetString(data.Array);
        }

        public int PopInt() =>
            buffer.ReadInt();

        public short PopShort() =>
            buffer.ReadShort();

        public bool PopBoolean() =>
            buffer.ReadByte() == 1;

        public int RemainingLength() =>
            buffer.ReadableBytes;

        public byte[] ReadBytes(int length)
        {
            return buffer.ReadBytes(length).Array;
        }
    }
}