
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class GetCatalogOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int OfferId = Packet.PopInt();
            if (!StarBlueServer.GetGame().GetCatalog().ItemOffers.ContainsKey(OfferId))
            {
                return;
            }

            int PageId = StarBlueServer.GetGame().GetCatalog().ItemOffers[OfferId];

            if (!StarBlueServer.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page))
            {
                return;
            }

            if (!Page.Enabled || !Page.Visible || (Page.MinimumRank > Session.GetHabbo().Rank && Page.MinimumVIP == 0) || (Page.MinimumVIP > 0 && Page.MinimumVIP > Session.GetHabbo().VIPRank && Page.MinimumRank > Session.GetHabbo().Rank))
            {
                return;
            }

            CatalogItem Item = null;
            if (!Page.ItemOffers.ContainsKey(OfferId))
            {
                return;
            }

            Item = Page.ItemOffers[OfferId];
            if (Item != null)
            {
                Session.SendMessage(new CatalogOfferComposer(Item));
            }
        }
    }
}
