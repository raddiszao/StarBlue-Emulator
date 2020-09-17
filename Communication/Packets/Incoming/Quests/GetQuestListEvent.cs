using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Quests
{
    public class GetQuestListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            StarBlueServer.GetGame().GetQuestManager().GetList(Session, null);
        }
    }
}