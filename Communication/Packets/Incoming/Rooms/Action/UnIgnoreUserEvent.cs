using StarBlue.Communication.Packets.Outgoing.Rooms.Action;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class UnIgnoreUserEvent : IPacketEvent
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
            if (player == null)
            {
                return;
            }

            if (!session.GetHabbo().GetIgnores().TryGet(player.Id))
            {
                return;
            }

            if (session.GetHabbo().GetIgnores().TryRemove(player.Id))
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `user_ignores` WHERE `user_id` = @uid AND `ignore_id` = @ignoreId");
                    dbClient.AddParameter("uid", session.GetHabbo().Id);
                    dbClient.AddParameter("ignoreId", player.Id);
                    dbClient.RunQuery();
                }

                session.SendMessage(new IgnoreStatusComposer(3, player.Username));
            }
        }
    }
}
