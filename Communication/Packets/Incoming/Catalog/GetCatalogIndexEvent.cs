
using StarBlue.Communication.Packets.Outgoing.BuildersClub;
using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.GameClients;
using StarBlue.Messages;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class GetCatalogIndexEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            QueuedServerMessage message = new QueuedServerMessage(Session.GetConnection());
            message.appendResponse(new CatalogIndexComposer(Session, StarBlueServer.GetGame().GetCatalog().GetPages(), "NORMAL"));
            //message.appendResponse(new CatalogIndexComposer(Session, StarBlueServer.GetGame().GetCatalog().GetBCPages(), "BUILDERS_CLUB"));

            message.appendResponse(new CatalogItemDiscountComposer());
            message.appendResponse(new BCBorrowedItemsComposer());
            message.sendResponse();
        }
    }
}