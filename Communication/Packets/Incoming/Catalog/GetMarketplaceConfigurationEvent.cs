using StarBlue.Communication.Packets.Outgoing.Catalog;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    public class GetMarketplaceConfigurationEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new MarketplaceConfigurationComposer());
        }
    }
}