using StarBlue.HabboHotel.Rooms.Polls;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Polls
{
    internal class PollOfferComposer : MessageComposer
    {
        private RoomPoll poll { get; }

        public PollOfferComposer(RoomPoll poll)
            : base(Composers.PollOfferMessageComposer)
        {
            this.poll = poll;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(poll.Id);
            packet.WriteString(RoomPollTypeUtility.GetRoomPollType(poll.Type).ToUpper());
            packet.WriteString(poll.Headline);
            packet.WriteString(poll.Summary);
        }
    }
}