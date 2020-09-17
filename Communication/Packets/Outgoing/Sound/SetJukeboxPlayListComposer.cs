using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class SetJukeboxPlayListComposer : MessageComposer
    {
        private Room room { get; }

        public SetJukeboxPlayListComposer(Room room)
            : base(Composers.SetJukeboxPlayListMessageComposer)
        {
            this.room = room;
        }

        public override void Compose(Composer packet)
        {
            System.Collections.Generic.List<HabboHotel.Items.Item> items = room.GetTraxManager().Playlist;
            packet.WriteInteger(items.Count); //Capacity
            packet.WriteInteger(items.Count); //While items Songs Count

            foreach (HabboHotel.Items.Item item in items)
            {
                int.TryParse(item.ExtraData, out int musicid);
                packet.WriteInteger(item.Id);
                packet.WriteInteger(musicid);//EndWhile
            }
        }
    }
}