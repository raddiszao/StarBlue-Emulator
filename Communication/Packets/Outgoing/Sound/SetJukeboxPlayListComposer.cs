using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Sound
{
    internal class SetJukeboxPlayListComposer : ServerPacket
    {
        public SetJukeboxPlayListComposer(Room room)
            : base(ServerPacketHeader.SetJukeboxPlayListMessageComposer)
        {
            System.Collections.Generic.List<HabboHotel.Items.Item> items = room.GetTraxManager().Playlist;
            base.WriteInteger(items.Count); //Capacity
            base.WriteInteger(items.Count); //While items Songs Count

            foreach (HabboHotel.Items.Item item in items)
            {
                int.TryParse(item.ExtraData, out int musicid);
                base.WriteInteger(item.Id);
                base.WriteInteger(musicid);//EndWhile
            }
        }
    }
}