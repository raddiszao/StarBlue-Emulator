using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    class PromotableRoomsComposer : ServerPacket
    {
        public PromotableRoomsComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.PromotableRoomsMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(Rooms.Count);//Count

            foreach (RoomData Data in Rooms)
            {
                base.WriteInteger(Data.Id);
                base.WriteString(Data.Name);
                base.WriteBoolean(false);
            }
        }
    }
}