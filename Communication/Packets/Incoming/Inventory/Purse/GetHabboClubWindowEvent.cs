using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Inventory.Purse
{
    internal class GetHabboClubWindowEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            CatalogPage page = StarBlueServer.GetGame().GetCatalog().TryGetPageByTemplate("vip_buy");
            if (page == null)
            {
                return;
            }

            Session.SendMessage(new GetClubComposer(page, Packet, Session));
        }
    }
}
