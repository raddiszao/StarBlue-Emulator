using StarBlue.Communication.Packets.Outgoing.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    class GetCatalogModeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string PageMode = Packet.PopString();

            if (PageMode == "NORMAL")
            {
                Session.SendMessage(new CatalogIndexComposer(Session, StarBlueServer.GetGame().GetCatalog().GetPages(), PageMode));//, Sub));
            }
            else if (PageMode == "BUILDERS_CLUB")
            {
                Session.SendMessage(new CatalogIndexComposer(Session, StarBlueServer.GetGame().GetCatalog().GetBCPages(), PageMode));
            }
        }
    }
}
