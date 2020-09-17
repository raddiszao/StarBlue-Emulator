using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class GetCatalogRoomPromotionComposer : MessageComposer
    {
        private List<RoomData> UsersRooms { get; }

        public GetCatalogRoomPromotionComposer(List<RoomData> UsersRooms)
            : base(Composers.PromotableRoomsMessageComposer)
        {
            this.UsersRooms = UsersRooms;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(true);//wat
            packet.WriteInteger(UsersRooms.Count);//Count of rooms?
            foreach (RoomData Room in UsersRooms)
            {
                packet.WriteInteger(Room.Id);
                packet.WriteString(Room.Name);
                packet.WriteBoolean(true);
            }
        }
    }
}
