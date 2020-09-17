using StarBlue.HabboHotel.Items;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class LoadJukeboxUserMusicItemsComposer : MessageComposer
    {
        private Room room { get; }

        public LoadJukeboxUserMusicItemsComposer(Room room)
            : base(Composers.LoadJukeboxUserMusicItemsMessageComposer)
        {
            this.room = room;
        }

        public override void Compose(Composer packet)
        {
            List<Item> songs = room.GetTraxManager().GetAvaliableSongs();

            packet.WriteInteger(songs.Count);//while
            foreach (Item item in songs)
            {
                packet.WriteInteger(item.Id);//item id
                packet.WriteInteger(item.ExtradataInt);//Song id
            }
        }
    }
}