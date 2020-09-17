using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Moderation;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class CloseTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            int Result = Packet.PopInt(); // 1 = useless, 2 = abusive, 3 = resolved
            int Junk = Packet.PopInt();
            int TicketId = Packet.PopInt();

            if (!StarBlueServer.GetGame().GetModerationManager().TryGetTicket(TicketId, out ModerationTicket Ticket))
            {
                return;
            }

            if (Ticket.Moderator.Id != Session.GetHabbo().Id)
            {
                return;
            }

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Ticket.Sender.Id);
            if (Client != null)
            {
                Client.SendMessage(new ModeratorSupportTicketResponseComposer(Result));
            }

            if (Result == 2)
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE `user_info` SET `cfhs_abusive` = `cfhs_abusive` + 1 WHERE `user_id` = '" + Ticket.Sender.Id + "' LIMIT 1");
                }
            }

            Ticket.Answered = true;
            StarBlueServer.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Ticket), "mod_tool");
        }
    }
}