using StarBlue.Communication.Packets.Outgoing.Rooms.Action;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    class IgnoreUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            String Username = Packet.PopString();
            Habbo User = StarBlueServer.GetHabboByUsername(Username);
            if (User == null || Session.GetHabbo().MutedUsers.Contains(User.Id) || User.GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            Session.GetHabbo().MutedUsers.Add(User.Id);
            Session.SendMessage(new IgnoreStatusComposer(1, Username));

            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModIgnoreSeen", 1);
        }
    }
}
