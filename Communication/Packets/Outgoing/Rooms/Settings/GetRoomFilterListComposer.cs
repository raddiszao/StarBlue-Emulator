
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class GetRoomFilterListComposer : ServerPacket
    {
        public GetRoomFilterListComposer(Room Instance)
            : base(ServerPacketHeader.GetRoomFilterListMessageComposer)
        {
            base.WriteInteger(Instance.WordFilterList.Count);
            foreach (string Word in Instance.WordFilterList)
            {
                base.WriteString(Word);
            }
        }
    }
}
