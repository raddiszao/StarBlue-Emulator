using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Generic;

namespace StarBlue.HabboHotel.Support
{
    /// <summary>
    /// TODO: Utilize ModerationTicket.cs
    /// </summary>
    public class SupportTicket
    {
        public readonly int Id;
        public readonly string ReportedName;
        public readonly string SenderName;

        public string Message;
        public string ModName;
        public int ModeratorId;
        public int ReportedId;

        public int RoomId;
        public string RoomName;
        public int Score;
        public int SenderId;
        public TicketStatus Status;

        public int Type;
        public double Timestamp;
        public List<string> ReportedChats;


        public SupportTicket(int Id, int Score, int Type, int SenderId, int ReportedId, String Message, int RoomId, String RoomName, Double Timestamp, List<string> ReportedChats)
        {
            this.Id = Id;
            this.Score = Score;
            this.Type = Type;
            Status = TicketStatus.OPEN;
            this.SenderId = SenderId;
            this.ReportedId = ReportedId;
            ModeratorId = 0;
            this.Message = Message;
            this.RoomId = RoomId;
            this.RoomName = RoomName;
            this.Timestamp = Timestamp;
            this.ReportedChats = ReportedChats;


            SenderName = StarBlueServer.GetGame().GetClientManager().GetNameById(SenderId);
            ReportedName = StarBlueServer.GetGame().GetClientManager().GetNameById(ReportedId);
            ModName = StarBlueServer.GetGame().GetClientManager().GetNameById(ModeratorId);
        }

        public int TabId
        {
            get
            {
                if (Status == TicketStatus.OPEN)
                {
                    return 1;
                }

                if (Status == TicketStatus.PICKED)
                {
                    return 2;
                }

                if (Status == TicketStatus.ABUSIVE || Status == TicketStatus.INVALID || Status == TicketStatus.RESOLVED)
                {
                    return 0;
                }

                if (Status == TicketStatus.DELETED)
                {
                    return 0;
                }

                return 0;
            }
        }

        public int TicketId => Id;

        public void Pick(int pModeratorId, Boolean UpdateInDb)
        {
            Status = TicketStatus.PICKED;
            ModeratorId = pModeratorId;
            ModName = StarBlueServer.GetHabboById(pModeratorId).Username;
            if (UpdateInDb)
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunFastQuery("UPDATE moderation_tickets SET status = 'picked', moderator_id = " + pModeratorId + ", timestamp = '" + StarBlueServer.GetUnixTimestamp() + "' WHERE id = " + Id + "");
                }
            }
        }

        public void Close(TicketStatus NewStatus)
        {
            Status = NewStatus;

            String dbType = "";

            switch (NewStatus)
            {
                case TicketStatus.ABUSIVE:

                    dbType = "abusive";
                    break;

                case TicketStatus.INVALID:

                    dbType = "invalid";
                    break;

                case TicketStatus.RESOLVED:
                default:

                    dbType = "resolved";
                    break;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE moderation_tickets SET status = '" + dbType + "' WHERE id = " + Id + " LIMIT 1");
            }

        }

        public void Release()
        {
            Status = TicketStatus.OPEN;


            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunFastQuery("UPDATE moderation_tickets SET status = 'open' WHERE id = " + Id + " LIMIT 1");
            }

        }
    }
}