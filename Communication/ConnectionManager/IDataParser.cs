using System;

namespace StarBlue.Communication.ConnectionManager
{
    public interface IDataParser : IDisposable, ICloneable
    {
        void HandlePacketData(byte[] packet);
    }
}