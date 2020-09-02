﻿namespace StarBlue.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListNotificationComposer : ServerPacket
    {
        public FurniListNotificationComposer(int Id, int Type)
            : base(ServerPacketHeader.FurniListNotificationMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(Type);
            base.WriteInteger(1);
            base.WriteInteger(Id);
        }
    }
}