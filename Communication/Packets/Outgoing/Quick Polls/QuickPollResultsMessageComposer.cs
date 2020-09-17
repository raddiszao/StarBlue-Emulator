namespace StarBlue.Communication.Packets.Outgoing.Rooms.Poll
{
    internal class QuickPollResultsMessageComposer : MessageComposer
    {
        private int yesVotesCount { get; }
        private int noVotesCount { get; }

        public QuickPollResultsMessageComposer(int yesVotesCount, int noVotesCount)
            : base(Composers.QuickPollResultsMessageComposer)
        {
            this.yesVotesCount = yesVotesCount;
            this.noVotesCount = noVotesCount;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(-1);
            packet.WriteInteger(2);
            packet.WriteString("1");
            packet.WriteInteger(yesVotesCount);
            packet.WriteString("0");
            packet.WriteInteger(noVotesCount);
        }
    }
}