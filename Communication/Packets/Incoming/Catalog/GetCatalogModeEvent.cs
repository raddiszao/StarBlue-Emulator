using StarBlue.Communication.Packets.Outgoing.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class GetCatalogModeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            string PageMode = Packet.PopString();

            if (PageMode == "NORMAL")
            {
                Session.SendMessage(new CatalogIndexComposer(Session, StarBlueServer.GetGame().GetCatalog().GetPages(), PageMode));//, Sub));
            }
        }
    }
}
