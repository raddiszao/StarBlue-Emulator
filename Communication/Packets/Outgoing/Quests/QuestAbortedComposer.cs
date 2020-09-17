namespace StarBlue.Communication.Packets.Outgoing.Quests
{
    internal class QuestAbortedComposer : MessageComposer
    {
        public QuestAbortedComposer()
            : base(Composers.QuestAbortedMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(false);
        }
    }
}
