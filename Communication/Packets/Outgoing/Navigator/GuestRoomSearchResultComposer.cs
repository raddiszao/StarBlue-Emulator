using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    class GuestRoomSearchResultComposer : ServerPacket
    {
        public GuestRoomSearchResultComposer(int Mode, string UserQuery, ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.GuestRoomSearchResultMessageComposer)
        {
            base.WriteInteger(Mode);
            base.WriteString(UserQuery);

            base.WriteInteger(Rooms.Count);
            foreach (RoomData data in Rooms)
            {
                RoomAppender.WriteRoom(this, data, data.Promotion);
            }

            base.WriteBoolean(false);
        }
    }
}
