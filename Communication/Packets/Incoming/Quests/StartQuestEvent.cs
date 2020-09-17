namespace StarBlue.Communication.Packets.Incoming.Quests
{
    internal class StartQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            int QuestId = Packet.PopInt();

            StarBlueServer.GetGame().GetQuestManager().ActivateQuest(Session, QuestId);
        }
    }
}
