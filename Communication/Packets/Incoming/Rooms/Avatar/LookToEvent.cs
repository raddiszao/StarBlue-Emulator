using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.PathFinding;

namespace StarBlue.Communication.Packets.Incoming.Rooms.Avatar
{
    internal class LookToEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (User.IsAsleep)
            {
                return;
            }

            User.UnIdle();

            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if ((X == User.X && Y == User.Y) || User.IsWalking || User.RidingHorse)
            {
                return;
            }

            int Rot = Rotation.Calculate(User.X, User.Y, X, Y);

            User.SetRot(Rot, false);
            User.UpdateNeeded = true;

            if (User.RidingHorse)
            {
                RoomUser Horse = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByVirtualId(User.HorseID);
                if (Horse != null)
                {
                    Horse.SetRot(Rot, false);
                    Horse.UpdateNeeded = true;
                }
            }

        }
    }
}
