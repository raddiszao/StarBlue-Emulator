namespace StarBlue.Communication.Packets.Outgoing.Rooms.Poll
{
    internal class QuickPollResultsMessageComposer : ServerPacket
    {
        public QuickPollResultsMessageComposer(int yesVotesCount, int noVotesCount)
            : base(ServerPacketHeader.QuickPollResultsMessageComposer)
        {
            base.WriteInteger(-1);
            base.WriteInteger(2);
            base.WriteString("1");
            base.WriteInteger(yesVotesCount);

            base.WriteString("0");
            base.WriteInteger(noVotesCount);
        }
    }
}