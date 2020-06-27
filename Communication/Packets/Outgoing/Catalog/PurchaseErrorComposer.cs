﻿namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    class PurchaseErrorComposer : ServerPacket
    {
        public PurchaseErrorComposer(int ErrorCode)
            : base(ServerPacketHeader.PurchaseErrorMessageComposer)
        {
            base.WriteInteger(ErrorCode);
        }
    }
}
