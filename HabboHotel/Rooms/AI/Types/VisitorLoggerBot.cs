using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Data;

namespace StarBlue.HabboHotel.Rooms.AI.Types
{
    internal class VisitorLogger : BotAI
    {
        private int VirtualId;

        public VisitorLogger(int VirtualId)
        {
            this.VirtualId = VirtualId;
        }

        public override void OnSelfEnterRoom()
        {
        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
            if (GetBotData() == null || User == null)
            {
                return;
            }

            RoomUser Bot = GetRoomUser();

            if (User.GetClient().GetHabbo().CurrentRoom.RoomData.OwnerId == User.GetClient().GetHabbo().Id)
            {
                DataTable getUsername;
                using (IQueryAdapter query = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    query.SetQuery("SELECT username FROM room_visits WHERE roomid = @id");
                    query.AddParameter("id", User.RoomId);
                    getUsername = query.GetTable();
                }

                foreach (DataRow Row in getUsername.Rows)
                {
                    Bot.Chat("Fico feliz em vê-lo senhor! Diga 'Sim', se você quiser saber quem visitou a sala na sua ausência.", false);
                    return;
                }

                Bot.Chat("Tenho sido muito atencioso(a) e garanto que ninguém visitou esta sala enquanto você estava fora.", false);
            }
            else
            {
                Bot.Chat("Olá " + User.GetClient().GetHabbo().Username + ", irei falar sobre você para o dono.", false);

                using (IQueryAdapter query = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    query.SetQuery("INSERT INTO room_visits (roomid, username, gone) VALUE (@roomid, @username, @gone)");
                    query.AddParameter("roomid", User.RoomId);
                    query.AddParameter("username", User.GetClient().GetHabbo().Username);
                    query.AddParameter("gone", "ainda está aqui.");
                    query.RunQuery();
                }
                return;
            }
        }


        public override void OnUserLeaveRoom(GameClient Client)
        {
            if (GetBotData() == null)
            {
                return;
            }

            RoomUser Bot = GetRoomUser();

            if (Client.GetHabbo().CurrentRoom.RoomData.OwnerId == Client.GetHabbo().Id)
            {
                DataTable getRoom;

                using (IQueryAdapter query = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    query.SetQuery("DELETE FROM room_visits WHERE roomid = @id");
                    query.AddParameter("id", Client.GetHabbo().CurrentRoom.Id);
                    getRoom = query.GetTable();
                }
            }
            DataTable getUpdate;

            using (IQueryAdapter query = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                query.SetQuery("UPDATE room_visits SET gone = @gone WHERE roomid = @id AND username = @username");
                query.AddParameter("gone", "esteve aqui.");
                query.AddParameter("id", Client.GetHabbo().CurrentRoom.Id);
                query.AddParameter("username", Client.GetHabbo().Username);
                getUpdate = query.GetTable();
            }
        }

        public override void OnUserSay(RoomUser User, string Message)
        {
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
            {
                return;
            }

            if (Gamemap.TileDistance(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y) > 8)
            {
                return;
            }

            switch (Message.ToLower())
            {
                case "yes":
                case "sim":
                    if (GetBotData() == null)
                    {
                        return;
                    }

                    if (User.GetClient().GetHabbo().CurrentRoom.RoomData.OwnerId == User.GetClient().GetHabbo().Id)
                    {
                        DataTable getRoomVisit;

                        using (IQueryAdapter query = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            query.SetQuery("SELECT username, gone FROM room_visits WHERE roomid = @id");
                            query.AddParameter("id", User.RoomId);
                            getRoomVisit = query.GetTable();
                        }

                        foreach (DataRow Row in getRoomVisit.Rows)
                        {
                            string gone = Convert.ToString(Row["gone"]);
                            string username = Convert.ToString(Row["username"]);

                            GetRoomUser().Chat(username + " " + gone, false);
                        }
                        using (IQueryAdapter query = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {
                            query.SetQuery("DELETE FROM room_visits WHERE roomid = @id");
                            query.AddParameter("id", User.RoomId);
                            getRoomVisit = query.GetTable();
                        }
                        return;
                    }
                    break;
            }
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {
        }
    }
}