namespace StarBlue.Communication.Packets.Incoming.Quests
{
    class GetCurrentQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            StarBlueServer.GetGame().GetQuestManager().GetCurrentQuest(Session, Packet);
        }
    }
}
