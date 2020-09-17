using StarBlue.Communication.Packets.Outgoing.Rooms.Polls;
using StarBlue.HabboHotel.Rooms.Polls;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Polls
{
    internal class PollStartEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, MessageEvent packet)
        {
            int pollId = packet.PopInt();

            if (!StarBlueServer.GetGame().GetPollManager().TryGetPoll(pollId, out RoomPoll poll))
            {
                return;
            }

            session.SendMessage(new PollContentsComposer(poll));
        }
    }
}
