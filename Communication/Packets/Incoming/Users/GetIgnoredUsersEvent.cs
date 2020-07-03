using StarBlue.Communication.Packets.Outgoing.Users;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Users
{
    class GetIgnoredUsersEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            List<string> ignoredUsers = new List<string>();

            foreach (int userId in new List<int>(session.GetHabbo().GetIgnores().IgnoredUserIds()))
            {
                Habbo player = StarBlueServer.GetHabboById(userId);
                if (player != null)
                {
                    if (!ignoredUsers.Contains(player.Username))
                    {
                        ignoredUsers.Add(player.Username);
                    }
                }
            }

            session.SendMessage(new IgnoredUsersComposer(ignoredUsers));
        }
    }
}