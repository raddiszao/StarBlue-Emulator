using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Rooms.Action;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    class IgnoreUserEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
            {
                return;
            }

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
            {
                return;
            }

            string username = packet.PopString();

            Habbo player = StarBlueServer.GetHabboByUsername(username);
            if (player == null || player.GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            if (session.GetHabbo().GetIgnores().TryGet(player.Id))
            {
                return;
            }

            if (session.GetHabbo().GetIgnores().TryAdd(player.Id))
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_ignores` (`user_id`,`ignore_id`) VALUES(@uid,@ignoreId);");
                    dbClient.AddParameter("uid", session.GetHabbo().Id);
                    dbClient.AddParameter("ignoreId", player.Id);
                    dbClient.RunQuery();
                }

                session.SendMessage(new IgnoreStatusComposer(1, player.Username));

                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_SelfModIgnoreSeen", 1);
            }
        }
    }
}
