using StarBlue.Communication.Packets.Outgoing.Rooms.Polls;
using StarBlue.HabboHotel.Rooms.Polls;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Polls
{
    class PollStartEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient session, ClientPacket packet)
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
