﻿namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    class RefreshFavouriteGroupComposer : ServerPacket
    {
        public RefreshFavouriteGroupComposer(int Id)
            : base(ServerPacketHeader.RefreshFavouriteGroupMessageComposer)
        {
            base.WriteInteger(Id);
        }
    }
}
