﻿
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    class MuteUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            int UserId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            int Time = Packet.PopInt();

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
            {
                return;
            }

            if (((Room.WhoCanMute == 0 && !Room.CheckRights(Session, true) && Room.Group == null) || (Room.WhoCanMute == 1 && !Room.CheckRights(Session)) && Room.Group == null) || (Room.Group != null && !Room.CheckRights(Session, false, true)))
            {
                return;
            }

            RoomUser Target = Room.GetRoomUserManager().GetRoomUserByHabbo(StarBlueServer.GetUsernameById(UserId));
            if (Target == null)
            {
                return;
            }
            else if (Target.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return;
            }

            if (Room.MutedUsers.ContainsKey(UserId))
            {
                if (Room.MutedUsers[UserId] < StarBlueServer.GetUnixTimestamp())
                {
                    Room.MutedUsers.Remove(UserId);
                }
                else
                {
                    return;
                }
            }

            Room.MutedUsers.Add(UserId, (StarBlueServer.GetUnixTimestamp() + (Time * 60)));

            Target.GetClient().SendWhisper("Você foi silenciado por " + Time + " minutos!");
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModMuteSeen", 1);
        }
    }
}
