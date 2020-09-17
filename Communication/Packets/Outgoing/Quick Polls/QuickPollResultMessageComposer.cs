namespace StarBlue.Communication.Packets.Outgoing.Rooms.Poll
{
    internal class QuickPollResultMessageComposer : MessageComposer
    {
        private int UserId { get; }
        private string myVote { get; }
        private int yesVotesCount { get; }
        private int noVotesCount { get; }

        public QuickPollResultMessageComposer(int UserId, string myVote, int yesVotesCount, int noVotesCount)
            : base(Composers.QuickPollResultMessageComposer)
        {
            this.UserId = UserId;
            this.myVote = myVote;
            this.yesVotesCount = yesVotesCount;
            this.noVotesCount = noVotesCount;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteString(myVote);
            packet.WriteInteger(2);
            packet.WriteString("1");
            packet.WriteInteger(yesVotesCount);

            packet.WriteString("0");
            packet.WriteInteger(noVotesCount);
        }
    }
}