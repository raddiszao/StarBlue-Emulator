using StarBlue.HabboHotel.Rooms.Polls;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Polls
{
    internal class PollOfferComposer : ServerPacket
    {
        public PollOfferComposer(RoomPoll poll)
            : base(ServerPacketHeader.PollOfferMessageComposer)
        {
            base.WriteInteger(poll.Id);
            base.WriteString(RoomPollTypeUtility.GetRoomPollType(poll.Type).ToUpper());
            base.WriteString(poll.Headline);
            base.WriteString(poll.Summary);
        }
    }
}