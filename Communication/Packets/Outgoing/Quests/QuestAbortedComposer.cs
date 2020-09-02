namespace StarBlue.Communication.Packets.Outgoing.Quests
{
    internal class QuestAbortedComposer : ServerPacket
    {
        public QuestAbortedComposer()
            : base(ServerPacketHeader.QuestAbortedMessageComposer)
        {
            base.WriteBoolean(false);
        }
    }
}
