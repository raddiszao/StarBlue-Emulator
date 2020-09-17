using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class GuestRoomSearchResultComposer : MessageComposer
    {
        private int Mode { get; }
        private string UserQuery { get; }
        private ICollection<RoomData> Rooms { get; }

        public GuestRoomSearchResultComposer(int Mode, string UserQuery, ICollection<RoomData> Rooms)
            : base(Composers.GuestRoomSearchResultMessageComposer)
        {
            this.Mode = Mode;
            this.UserQuery = UserQuery;
            this.Rooms = Rooms;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Mode);
            packet.WriteString(UserQuery);

            packet.WriteInteger(Rooms.Count);
            foreach (RoomData data in Rooms)
            {
                RoomAppender.WriteRoom(packet, data, data.Promotion);
            }

            packet.WriteBoolean(false);
        }
    }
}
