using StarBlue.Communication.Packets.Outgoing.LandingView;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    class CommunityGoalEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new CommunityGoalComposer());
            Session.SendMessage(new DynamicPollLandingComposer(false)); //false pa q pueda votar
        }
    }
}
