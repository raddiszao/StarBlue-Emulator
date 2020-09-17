
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogPageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            int PageId = Packet.PopInt();
            int Something = Packet.PopInt();
            string CataMode = Packet.PopString();

            CatalogPage Page = null;
            //BCCatalogPage BCPage = null;

            if (CataMode == "NORMAL")
            {
                if (!StarBlueServer.GetGame().GetCatalog().TryGetPage(PageId, out Page))
                {
                    return;
                }

                if (!Page.Enabled || !Page.Visible || (Page.MinimumRank > Session.GetHabbo().Rank && Page.MinimumVIP == 0) || (Page.MinimumVIP > 0 && Page.MinimumVIP > Session.GetHabbo().VIPRank && Page.MinimumRank > Session.GetHabbo().Rank))
                {
                    return;
                }

                Session.SendMessage(new CatalogPageComposer(Page, CataMode, Session));
            }

            /*if (CataMode == "BUILDERS_CLUB")
            {
                if (!StarBlueServer.GetGame().GetCatalog().TryGetBCPage(PageId, out BCPage))
                {
                    return;
                }

                if (!BCPage.Enabled || !BCPage.Visible || BCPage.MinimumRank > Session.GetHabbo().Rank || (BCPage.MinimumVIP >= Session.GetHabbo().VIPRank && Session.GetHabbo().Rank == 2))
                {
                    return;
                }

                Session.SendMessage(new BCCatalogPageComposer(BCPage, CataMode));
            }*/

        }
    }
}