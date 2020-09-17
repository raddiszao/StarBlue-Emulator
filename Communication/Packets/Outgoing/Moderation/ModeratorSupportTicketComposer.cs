using StarBlue.HabboHotel.Moderation;
using StarBlue.Utilities;
using System;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorSupportTicketComposer : MessageComposer
    {
        private ModerationTicket Ticket { get; }

        public ModeratorSupportTicketComposer(ModerationTicket Ticket)
          : base(Composers.ModeratorSupportTicketMessageComposer)
        {
            this.Ticket = Ticket;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Ticket.Id); // Id
            packet.WriteInteger(Ticket.GetStatus(Header)); // Tab ID
            packet.WriteInteger(Ticket.Type); // Type
            packet.WriteInteger(Ticket.Category); // Category 
            packet.WriteInteger(Convert.ToInt32((DateTime.Now - UnixTimestamp.FromUnixTimestamp(Ticket.Timestamp)).TotalMilliseconds)); // Timestamp
            packet.WriteInteger(Ticket.Priority); // Priority
            packet.WriteInteger(0); //
            packet.WriteInteger(Ticket.Sender == null ? 0 : Ticket.Sender.Id); // Sender ID
                                                                               //packet.WriteInteger(1);
            packet.WriteString(Ticket.Sender == null ? string.Empty : Ticket.Sender.Username); // Sender Name
            packet.WriteInteger(Ticket.Reported == null ? 0 : Ticket.Reported.Id); // Reported ID
            packet.WriteString(Ticket.Reported == null ? string.Empty : Ticket.Reported.Username); // Reported Name
            packet.WriteInteger(Ticket.Moderator == null ? 0 : Ticket.Moderator.Id); // Moderator ID
            packet.WriteString(Ticket.Moderator == null ? string.Empty : Ticket.Moderator.Username); // Mod Name
            packet.WriteString(Ticket.Issue); // Issue
            packet.WriteInteger(Ticket.Room == null ? 0 : Ticket.Room.Id); // Room Id
            packet.WriteInteger(0);
            {
            }
        }
    }
}