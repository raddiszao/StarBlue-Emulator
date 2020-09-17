using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.HabboHotel.Moderation;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class ReleaseTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int Amount = Packet.PopInt();

            for (int i = 0; i < Amount; i++)
            {
                if (!StarBlueServer.GetGame().GetModerationManager().TryGetTicket(Packet.PopInt(), out ModerationTicket Ticket))
                {
                    continue;
                }

                Ticket.Moderator = null;
                StarBlueServer.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Ticket), "mod_tool");
            }
        }
    }
}