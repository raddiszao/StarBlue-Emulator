﻿namespace StarBlue.Communication.Packets.Outgoing.Misc
{
    class LatencyTestComposer : ServerPacket
    {
        public LatencyTestComposer(int testResponce)
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {
            base.WriteInteger(testResponce);
        }
    }
}
