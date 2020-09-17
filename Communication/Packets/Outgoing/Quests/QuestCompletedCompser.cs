namespace StarBlue.Communication.Packets.Outgoing.Quests
{
    internal class QuestCompletedCompser : MessageComposer
    {
        public QuestCompletedCompser()
            : base(Composers.QuestCompletedMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {

        }
    }
}