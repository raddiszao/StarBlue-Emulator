using DotNetty.Buffers;
using DotNetty.Common;
using System.Text;

namespace StarBlue.Communication.Packets.Outgoing
{
    public class Composer : IByteBufferHolder
    {
        protected IByteBuffer Buffer { get; }
        protected short Header { get; }

        public Composer(short id, IByteBuffer body)
        {
            Buffer = body;
            Header = id;
            if (body.WriterIndex == 0)
            {
                Buffer.WriteInt(-1);
                Buffer.WriteShort(id);
            }
        }

        public void WriteByte(byte b) =>
            Buffer.WriteByte(b);

        public void WriteByte(int b) =>
            Buffer.WriteByte((byte)b);

        public void WriteDouble(double d) =>
            WriteString(d.ToString());

        public void WriteString(string s) // d
        {
            Buffer.WriteShort(s.Length);
            Buffer.WriteBytes(Encoding.Default.GetBytes(s));
        }

        public void WriteShort(int s) =>
            Buffer.WriteShort(s);

        public void WriteInteger(int i) =>
            Buffer.WriteInt(i);

        public void WriteBoolean(bool b) =>
            Buffer.WriteByte(b ? 1 : 0);

        public int Length => Buffer.WriterIndex - 4;

        public IByteBuffer Content => Buffer;

        public int ReferenceCount => Buffer.ReferenceCount;

        public bool IsFinalized() => (Buffer.GetInt(0) > -1);

        public IByteBufferHolder Copy()
        {
            return new Composer(Header, Buffer.Copy());
        }

        public IByteBufferHolder Duplicate()
        {
            return new Composer(Header, Buffer.Duplicate());
        }

        public IByteBufferHolder RetainedDuplicate()
        {
            return new Composer(Header, Buffer.RetainedDuplicate());
        }

        public IByteBufferHolder Replace(IByteBuffer content)
        {
            return new Composer(Header, content);
        }

        public IReferenceCounted Retain()
        {
            return Buffer.Retain();
        }

        public IReferenceCounted Retain(int increment)
        {
            return Buffer.Retain(increment);
        }

        public IReferenceCounted Touch()
        {
            return Buffer.Touch();
        }

        public IReferenceCounted Touch(object hint)
        {
            return Buffer.Touch(hint);
        }

        public bool Release()
        {
            return Buffer.Release();
        }

        public bool Release(int decrement)
        {
            return Buffer.Release(decrement);
        }
    }
}