using StarBlue.Communication.Packets.Outgoing.LandingView;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    internal class CommunityGoalEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            Session.SendMessage(new CommunityGoalComposer());
            Session.SendMessage(new DynamicPollLandingComposer(false)); //false pa q pueda votar
        }
    }
}
