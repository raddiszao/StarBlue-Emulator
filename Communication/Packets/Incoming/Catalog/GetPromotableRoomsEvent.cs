using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class GetPromotableRoomsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            List<RoomData> Rooms = Session.GetHabbo().UsersRooms;
            Rooms = Rooms.Where(x => (x.Promotion == null || x.Promotion.TimestampExpires < StarBlueServer.GetUnixTimestamp())).ToList();
            Session.SendMessage(new PromotableRoomsComposer(Rooms));
        }
    }
}
