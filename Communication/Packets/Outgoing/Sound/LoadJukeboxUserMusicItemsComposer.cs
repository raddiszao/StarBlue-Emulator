using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    class LoadJukeboxUserMusicItemsComposer : ServerPacket
    {
        public LoadJukeboxUserMusicItemsComposer(Room room)
            : base(ServerPacketHeader.LoadJukeboxUserMusicItemsMessageComposer)
        {
            var songs = room.GetTraxManager().GetAvaliableSongs();

            base.WriteInteger(songs.Count);//while
            foreach (var item in songs)
            {
                base.WriteInteger(item.Id);//item id
                base.WriteInteger(item.ExtradataInt);//Song id
            }
        }

        public LoadJukeboxUserMusicItemsComposer(ICollection<Item> Items)
            : base(ServerPacketHeader.LoadJukeboxUserMusicItemsMessageComposer)
        {

            base.WriteInteger(Items.Count);//while
            foreach (var item in Items)
            {
                base.WriteInteger(item.Id);//item id
                base.WriteInteger(item.ExtradataInt);//Song id
            }
        }
    }
}