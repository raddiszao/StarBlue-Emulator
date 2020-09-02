using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.HabboHotel.Moderation;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class PickTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int Junk = Packet.PopInt();//??
            int TicketId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetModerationManager().TryGetTicket(TicketId, out ModerationTicket Ticket))
            {
                return;
            }

            Ticket.Moderator = Session.GetHabbo();
            StarBlueServer.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Session.GetHabbo().Id, Ticket), "mod_tool");
        }
    }
}
