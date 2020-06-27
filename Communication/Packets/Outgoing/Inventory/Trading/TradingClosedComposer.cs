﻿namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    class TradingClosedComposer : ServerPacket
    {
        public TradingClosedComposer(int UserId)
            : base(ServerPacketHeader.TradingClosedMessageComposer)
        {
            base.WriteInteger(UserId);
            base.WriteInteger(0);
        }
    }
}