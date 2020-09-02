
using StarBlue.HabboHotel.Quests;

namespace StarBlue.Communication.Packets.Incoming.Messenger
{
    internal class RequestBuddyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            if (Session.GetHabbo().GetMessenger().RequestBuddy(Packet.PopString()))
            {
                StarBlueServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_FRIEND);
            }
        }
    }
}
