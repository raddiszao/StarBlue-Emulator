namespace StarBlue.Communication.Packets.Incoming.Quests
{
    class CancelQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            StarBlueServer.GetGame().GetQuestManager().CancelQuest(Session, Packet);
        }
    }
}
