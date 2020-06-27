﻿using System;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    class UpdateMagicTileComposer : ServerPacket
    {
        public UpdateMagicTileComposer(int ItemId, int Decimal)
            : base(ServerPacketHeader.UpdateMagicTileMessageComposer)
        {
            base.WriteInteger(Convert.ToInt32(ItemId));
            base.WriteInteger(Decimal);
        }
    }
}
