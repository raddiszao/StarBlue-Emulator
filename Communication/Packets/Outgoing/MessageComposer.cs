using DotNetty.Buffers;

namespace StarBlue.Communication.Packets.Outgoing
{
    public abstract class MessageComposer
    {
        protected short Header { get; }

        public MessageComposer(short Id)
        {
            this.Header = Id;
        }

        public int GetId()
        {
            return this.Header;
        }

        public Composer WriteMessage(IByteBuffer buf)
        {
            Composer packet = new Composer(Header, buf);
            try
            {
                this.Compose(packet);
            }
            finally
            {
                this.Dispose();
            }
            return packet;
        }

        public abstract void Compose(Composer packet);

        public void Dispose()
        {

        }
    }
}