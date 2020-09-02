using StarBlue.Communication.Packets.Outgoing.Moderation;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Moderation;
using StarBlue.HabboHotel.Users;
using StarBlue.Utilities;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Moderation
{
    internal class SubmitNewTicketEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (StarBlueServer.GetGame().GetModerationManager().UserHasTickets(Session.GetHabbo().Id))
            {
                ModerationTicket PendingTicket = StarBlueServer.GetGame().GetModerationManager().GetTicketBySenderId(Session.GetHabbo().Id);
                if (PendingTicket != null)
                {
                    Session.SendMessage(new CallForHelpPendingCallsComposer(PendingTicket));
                    return;
                }
            }

            List<string> Chats = new List<string>();

            string Message = StringCharFilter.Escape(Packet.PopString().Trim());
            int Category = Packet.PopInt();
            int ReportedUserId = Packet.PopInt();
            int Type = Packet.PopInt();

            Habbo ReportedUser = StarBlueServer.GetHabboById(ReportedUserId);
            if (ReportedUser == null)
            {
                return;
            }

            int Messagecount = Packet.PopInt();
            for (int i = 0; i < Messagecount; i++)
            {
                Packet.PopInt();
                Chats.Add(Packet.PopString());
            }

            ModerationTicket Ticket = new ModerationTicket(1, Type, Category, UnixTimestamp.GetNow(), 1, Session.GetHabbo(), ReportedUser, Message, Session.GetHabbo().CurrentRoom.RoomData, Chats);
            if (!StarBlueServer.GetGame().GetModerationManager().TryAddTicket(Ticket))
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE `user_info` SET `cfhs` = `cfhs` + '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            StarBlueServer.GetGame().GetClientManager().ModAlert("A new support ticket has been submitted!");
            StarBlueServer.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Session.GetHabbo().Id, Ticket), "mod_tool");
        }
    }
}
