﻿using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    class SetJukeboxPlayListComposer : ServerPacket
    {
        public SetJukeboxPlayListComposer(Room room)
            : base(ServerPacketHeader.SetJukeboxPlayListMessageComposer)
        {
            var items = room.GetTraxManager().Playlist;
            base.WriteInteger(items.Count); //Capacity
            base.WriteInteger(items.Count); //While items Songs Count

            foreach (var item in items)
            {
                int.TryParse(item.ExtraData, out int musicid);
                base.WriteInteger(item.Id);
                base.WriteInteger(musicid);//EndWhile
            }
        }
    }
}