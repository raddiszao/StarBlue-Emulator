//using System;
//using System.Collections.Generic;

//using StarBlue.HabboHotel.Rooms;
//using StarBlue.HabboHotel.GameClients;

//using StarBlue.Database.Interfaces;

//using StarBlue.Communication.Packets.Outgoing.Moderation;
//using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;

//namespace StarBlue.HabboHotel.Support
//{   
//    /// <summary>
//    /// TODO: Utilize ModerationTicket.cs
//    /// </summary>
//    public class ModerationTool
//    {
//        public List<SupportTicket> Tickets;

//        public ModerationTool()
//        {
//            Tickets = new List<SupportTicket>();
//        }

//        public ICollection<SupportTicket> GetTickets
//        {
//            get { return this.Tickets; }
//        }


//        #region Support Tickets
//        public void SendNewTicket(GameClient Session, int Category, int ReportedUser, String Message, List<string> Messages)
//        {
//            int TicketId = 0;
//            SupportTicket Ticket;

//            if (Session.GetHabbo().CurrentRoomId <= 0)
//            {
//                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
//                {
//                    dbClient.SetQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + Category + "','open','" + Session.GetHabbo().Id + "','" + ReportedUser + "','0',@message,'0','','" + StarBlueServer.GetUnixTimestamp() + "')");
//                    dbClient.AddParameter("message", Message);
//                    TicketId = Convert.ToInt32(dbClient.InsertQuery());

//                    dbClient.RunFastQuery("UPDATE `user_info` SET `cfhs` = `cfhs` + '1' WHERE `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
//                }

//                Ticket = new SupportTicket(TicketId, 1, 7, Session.GetHabbo().Id, ReportedUser, Message, 0, "", StarBlueServer.GetUnixTimestamp(), Messages);

//                Tickets.Add(Ticket);

//                SendTicketToModerators(Ticket);
//                return;
//            }

//            RoomData Data = StarBlueServer.GetGame().GetRoomManager().GenerateRoomData(Session.GetHabbo().CurrentRoomId);

//            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
//            {
//                dbClient.SetQuery("INSERT INTO moderation_tickets (score,type,status,sender_id,reported_id,moderator_id,message,room_id,room_name,timestamp) VALUES (1,'" + Category + "','open','" + Session.GetHabbo().Id + "','" + ReportedUser + "','0',@message,'" + Data.Id + "',@name,'" + StarBlueServer.GetUnixTimestamp() + "')");
//                dbClient.AddParameter("message", Message);
//                dbClient.AddParameter("name", Data.Name);
//                TicketId = Convert.ToInt32(dbClient.InsertQuery());

//                dbClient.RunFastQuery("UPDATE user_info SET cfhs = cfhs + 1 WHERE user_id = '" + Session.GetHabbo().Id + "' LIMIT 1");
//            }

//            Ticket = new SupportTicket(TicketId, 1, 7, Session.GetHabbo().Id, ReportedUser, Message, Data.Id, Data.Name, StarBlueServer.GetUnixTimestamp(), Messages);
//            Tickets.Add(Ticket);
//            SendTicketToModerators(Ticket);
//        }

//        public SupportTicket GetTicket(int TicketId)
//        {
//            foreach (SupportTicket Ticket in Tickets)
//            {
//                if (Ticket.TicketId == TicketId)
//                {
//                    return Ticket;
//                }
//            }
//            return null;
//        }

//        public void PickTicket(GameClient Session, int TicketId)
//        {
//            SupportTicket Ticket = GetTicket(TicketId);

//            if (Ticket == null || Ticket.Status != TicketStatus.OPEN)
//            {
//                return;
//            }

//            Ticket.Pick(Session.GetHabbo().Id, true);
//            SendTicketToModerators(Ticket);
//        }

//        public void ReleaseTicket(GameClient Session, int TicketId)
//        {
//            SupportTicket Ticket = GetTicket(TicketId);

//            if (Ticket == null || Ticket.Status != TicketStatus.PICKED || Ticket.ModeratorId != Session.GetHabbo().Id)
//            {
//                return;
//            }

//            Ticket.Release();
//            SendTicketToModerators(Ticket);
//        }

//        public void CloseTicket(GameClient Session, int TicketId, int Result)
//        {
//            SupportTicket Ticket = GetTicket(TicketId);

//            if (Ticket == null || Ticket.Status != TicketStatus.PICKED || Ticket.ModeratorId != Session.GetHabbo().Id)
//            {
//                return;
//            }

//            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Ticket.SenderId);

//            TicketStatus NewStatus;
//            int ResultCode = 0;
//            switch (Result)
//            {
//                case 1:
//                    {
//                        ResultCode = 1;
//                        NewStatus = TicketStatus.INVALID;
//                    }
//                    break;

//                case 2:
//                    {
//                        ResultCode = 2;
//                        NewStatus = TicketStatus.ABUSIVE;

//                        using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
//                        {
//                            dbClient.RunFastQuery("UPDATE `user_info` SET `cfhs_abusive` = `cfhs_abusive` + 1 WHERE `user_id` = '" + Ticket.SenderId + "' LIMIT 1");
//                        }
//                    }
//                    break;

//                case 3:
//                default:
//                    {
//                        ResultCode = 0;
//                        NewStatus = TicketStatus.RESOLVED;
//                    }
//                    break;
//            }

//            if (Client != null)
//                Client.SendMessage(new ModeratorSupportTicketResponseComposer(ResultCode));

//            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
//            {
//                dbClient.RunFastQuery("UPDATE `user_stats` SET `tickets_answered` = `tickets_answered` + '1' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
//            }
//            Ticket.Close(NewStatus);
//            SendTicketToModerators(Ticket);
//        }

//        public bool UsersHasPendingTicket(int Id)
//        {
//            foreach (SupportTicket Ticket in Tickets)
//            {
//                if (Ticket.SenderId == Id && Ticket.Status == TicketStatus.OPEN)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }


//        public static void SendTicketToModerators(SupportTicket Ticket)
//        {
//            StarBlueServer.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(Ticket), "mod_tool");
//            StarBlueServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("tickets", "Hay una nueva petición de ayuda, revísala cuanto antes.", ""));

//        }

//        #endregion
//    }
//}