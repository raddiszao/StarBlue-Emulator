
using StarBlue.HabboHotel.Rooms;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerRemovedComposer : ServerPacket
    {
        public FlatControllerRemovedComposer(Room Instance, int UserId)
            : base(ServerPacketHeader.FlatControllerRemovedMessageComposer)
        {
            base.WriteInteger(Instance.Id);
            base.WriteInteger(UserId);
        }
    }
}
