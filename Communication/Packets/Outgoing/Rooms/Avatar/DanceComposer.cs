using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class DanceComposer : ServerPacket
    {
        public DanceComposer(RoomUser Avatar, int Dance)
            : base(ServerPacketHeader.DanceMessageComposer)
        {
            base.WriteInteger(Avatar.VirtualId);
            base.WriteInteger(Dance);
        }
    }
}