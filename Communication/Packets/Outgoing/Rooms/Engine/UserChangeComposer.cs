
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    class UserChangeComposer : ServerPacket
    {
        public UserChangeComposer(RoomUser User, bool Self)
            : base(ServerPacketHeader.UserChangeMessageComposer)
        {
            base.WriteInteger((Self) ? -1 : User.VirtualId);
            base.WriteString(User.GetClient().GetHabbo().Look);
            base.WriteString(User.GetClient().GetHabbo().Gender);
            base.WriteString(User.GetClient().GetHabbo().Motto);
            base.WriteInteger(User.GetClient().GetHabbo().GetStats().AchievementPoints);
        }
    }
}