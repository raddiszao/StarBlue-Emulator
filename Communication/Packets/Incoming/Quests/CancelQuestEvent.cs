namespace StarBlue.Communication.Packets.Incoming.Quests
{
    internal class CancelQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            StarBlueServer.GetGame().GetQuestManager().CancelQuest(Session, Packet);
        }
    }
}
