using StarBlue.Communication.Packets.Outgoing.Handshake;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Misc
{
    internal class GetAdsOfferEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new VideoOffersRewardsComposer());
        }
    }
}
