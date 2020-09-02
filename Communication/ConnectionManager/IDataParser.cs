using System;

namespace StarBlue.Communication.ConnectionManager
{
    public interface IDataParser : IDisposable, ICloneable
    {
        void handlePacketData(byte[] packet, bool _deciphered = false);
    }
}