
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Action
{
    internal class KickUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || Room.RoomData.WhoCanKick != 2 && (Room.RoomData.WhoCanKick != 1 || !Room.CheckRights(Session, false, true)) && !Room.CheckRights(Session, true))
                return;

            int UserId = Packet.PopInt();
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(UserId);
            if (User == null || User.IsBot)
            {
                return;
            }

            //Cannot kick owner or moderators.
            if (Room.CheckRights(User.GetClient(), true) || User.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") && Session.GetHabbo().Rank < User.GetClient().GetHabbo().Rank)
            {
                return;
            }

            Room.GetRoomUserManager().RemoveUserFromRoom(User.GetClient(), true, true);
            StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_SelfModKickSeen", 1);
        }
    }
}
