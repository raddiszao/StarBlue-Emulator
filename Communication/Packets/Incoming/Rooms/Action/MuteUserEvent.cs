
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class MuteUserEvent : IPacketEvent
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

            if (!Session.GetHabbo().GetPermissions().HasRight("mod_mute") && ((Room.RoomData.WhoCanMute == 0 && !Room.CheckRights(Session, true) && Room.RoomData.Group == null) || (Room.RoomData.WhoCanMute == 1 && !Room.CheckRights(Session)) && Room.RoomData.Group == null) || (Room.RoomData.Group != null && !Room.CheckRights(Session, false, true)))
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
                Room.MutedUsers.Remove(UserId);
                return;
            }

            Room.MutedUsers.Add(UserId, (StarBlueServer.GetUnixTimestamp() + (Time * 60)));

            Target.GetClient().SendWhisper("Você foi silenciado por " + Time + " minutos!", 34);
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModMuteSeen", 1);
        }
    }
}
