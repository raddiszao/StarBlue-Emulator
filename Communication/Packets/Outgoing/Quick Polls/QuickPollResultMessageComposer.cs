using System;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Poll
{
    class QuickPollResultMessageComposer : ServerPacket
    {
        public QuickPollResultMessageComposer(int UserId, String myVote, int yesVotesCount, int noVotesCount)
            : base(ServerPacketHeader.QuickPollResultMessageComposer)
        {
            base.WriteInteger(UserId);
            base.WriteString(myVote);
            base.WriteInteger(2);
            base.WriteString("1");
            base.WriteInteger(yesVotesCount);

            base.WriteString("0");
            base.WriteInteger(noVotesCount);
        }
    }
}