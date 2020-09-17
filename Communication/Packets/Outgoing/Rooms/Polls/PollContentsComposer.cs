using StarBlue.HabboHotel.Rooms.Polls;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Polls
{
    internal class PollContentsComposer : MessageComposer
    {
        private RoomPoll poll { get; }

        public PollContentsComposer(RoomPoll poll)
            : base(Composers.PollContentsMessageComposer)
        {
            this.poll = poll;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(poll.Id);
            packet.WriteString(poll.Headline);
            packet.WriteString(poll.CompletionMessage);

            packet.WriteInteger(poll.Questions.Count);
            foreach (RoomPollQuestion question in poll.Questions.Values)
            {
                packet.WriteInteger(question.Id);
                packet.WriteInteger(question.SeriesOrder);
                packet.WriteInteger(RoomPollQuestionTypeUtility.GetQuestionType(question.Type));
                packet.WriteString(question.Question);

                packet.WriteInteger(0); // ??
                packet.WriteInteger(question.MinimumSlections);// Min selections

                packet.WriteInteger(question.Selections.Count);
                foreach (RoomPollQuestionSelection Selection in question.Selections.Values)
                {
                    packet.WriteString(Selection.Value);
                    packet.WriteString(Selection.Text);
                    packet.WriteInteger(Selection.Id);
                }

                packet.WriteInteger(0);//??
            }
            packet.WriteBoolean(true);//No idea
        }
    }
}