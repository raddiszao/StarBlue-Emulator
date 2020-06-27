using StarBlue.Communication.Packets.Outgoing;
using StarBlue.HabboHotel.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Purse
{
    class GetHabboClubWindowEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            CatalogPage page = StarBlueServer.GetGame().GetCatalog().TryGetPageByTemplate("vip_buy");
            if (page == null)
            {
                return;
            }

            ServerPacket Message = new ServerPacket(ServerPacketHeader.GetClubComposer);
            Message.WriteInteger(page.Items.Values.Count);

            foreach (CatalogItem catalogItem in page.Items.Values)
            {
                catalogItem.SerializeClub(Message, Session);
            }

            Message.WriteInteger(Packet.PopInt());

            Session.SendMessage(Message);
        }
    }
}
