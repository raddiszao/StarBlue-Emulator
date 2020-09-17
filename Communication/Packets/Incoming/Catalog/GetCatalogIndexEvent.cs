
using StarBlue.Communication.Packets.Outgoing.BuildersClub;
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            Session.SendQueue(new CatalogIndexComposer(Session, StarBlueServer.GetGame().GetCatalog().GetPages(), "NORMAL"));
            //Session.SendMessage(new CatalogIndexComposer(Session, StarBlueServer.GetGame().GetCatalog().GetBCPages(), "BUILDERS_CLUB"));

            Session.SendQueue(new CatalogItemDiscountComposer());
            Session.SendQueue(new BCBorrowedItemsComposer());
            Session.Flush();
        }
    }
}