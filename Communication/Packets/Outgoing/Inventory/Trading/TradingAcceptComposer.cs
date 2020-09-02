﻿namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingAcceptComposer : ServerPacket
    {
        public TradingAcceptComposer(int UserId, bool Accept)
            : base(ServerPacketHeader.TradingAcceptMessageComposer)
        {
            base.WriteInteger(UserId);
            base.WriteInteger(Accept ? 1 : 0);
        }
    }
}
