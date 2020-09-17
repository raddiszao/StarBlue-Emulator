using StarBlue.HabboHotel.Moderation;
using StarBlue.Utilities;

namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class CallForHelpPendingCallsComposer : MessageComposer
    {
        private ModerationTicket ticket { get; }

        public CallForHelpPendingCallsComposer(ModerationTicket ticket)
            : base(Composers.CallForHelpPendingCallsMessageComposer)
        {
            this.ticket = ticket;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);// Count for whatever reason?
            {
                packet.WriteString(ticket.Id.ToString());
                packet.WriteString(UnixTimestamp.FromUnixTimestamp(ticket.Timestamp).ToShortTimeString());
                packet.WriteString(ticket.Issue);
            }
        }
    }
}
