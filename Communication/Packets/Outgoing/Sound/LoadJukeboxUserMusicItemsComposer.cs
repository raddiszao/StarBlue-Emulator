using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class LoadJukeboxUserMusicItemsComposer : ServerPacket
    {
        public LoadJukeboxUserMusicItemsComposer(Room room)
            : base(ServerPacketHeader.LoadJukeboxUserMusicItemsMessageComposer)
        {
            List<Item> songs = room.GetTraxManager().GetAvaliableSongs();

            base.WriteInteger(songs.Count);//while
            foreach (Item item in songs)
            {
                base.WriteInteger(item.Id);//item id
                base.WriteInteger(item.ExtradataInt);//Song id
            }
        }

        public LoadJukeboxUserMusicItemsComposer(ICollection<Item> Items)
            : base(ServerPacketHeader.LoadJukeboxUserMusicItemsMessageComposer)
        {

            base.WriteInteger(Items.Count);//while
            foreach (Item item in Items)
            {
                base.WriteInteger(item.Id);//item id
                base.WriteInteger(item.ExtradataInt);//Song id
            }
        }
    }
}