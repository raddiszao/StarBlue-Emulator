using StarBlue.Communication.Packets.Outgoing.Catalog;

namespace StarBlue.Communication.Packets.Incoming.Catalog
{
    internal class FurniMaticRewardsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {

            Session.SendMessage(new FurniMaticRewardsComposer());
        }
    }
}

