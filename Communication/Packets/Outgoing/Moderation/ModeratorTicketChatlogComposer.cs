using StarBlue.HabboHotel.Moderation;
using StarBlue.HabboHotel.Rooms;
using StarBlue.Utilities;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorTicketChatlogComposer : MessageComposer
    {
        private ModerationTicket ticket { get; }
        private RoomData roomData { get; }
        private double timestamp { get; }

        public ModeratorTicketChatlogComposer(ModerationTicket ticket, RoomData roomData, double timestamp)
              : base(Composers.ModeratorTicketChatlogMessageComposer)
        {
            this.ticket = ticket;
            this.roomData = roomData;
            this.timestamp = timestamp;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ticket.Id);
            packet.WriteInteger(ticket.Sender != null ? ticket.Sender.Id : 0);
            packet.WriteInteger(ticket.Reported != null ? ticket.Reported.Id : 0);
            packet.WriteInteger(roomData.Id);

            packet.WriteByte(1);
            packet.WriteShort(2);//Count
            packet.WriteString("roomName");
            packet.WriteByte(2);
            packet.WriteString(roomData.Name);
            packet.WriteString("roomId");
            packet.WriteByte(1);
            packet.WriteInteger(roomData.Id);

            packet.WriteShort(ticket.ReportedChats.Count);
            foreach (string Chat in ticket.ReportedChats)
            {
                packet.WriteString(UnixTimestamp.FromUnixTimestamp(timestamp).ToShortTimeString());
                packet.WriteInteger(ticket.Id);
                packet.WriteString(ticket.Reported != null ? ticket.Reported.Username : "No username");
                packet.WriteString(Chat);
                packet.WriteBoolean(false);
            }
        }
    }
}