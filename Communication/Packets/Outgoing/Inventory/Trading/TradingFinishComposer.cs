﻿namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingFinishComposer : ServerPacket
    {
        public TradingFinishComposer()
            : base(ServerPacketHeader.TradingFinishMessageComposer)
        {
        }
    }
}
