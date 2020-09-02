
using StarBlue.Communication.Packets.Outgoing.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class GetCatalogRoomPromotionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new GetCatalogRoomPromotionComposer(Session.GetHabbo().UsersRooms));
        }
    }
}
