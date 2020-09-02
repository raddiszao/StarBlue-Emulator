
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ObjectRemoveComposer : ServerPacket
    {
        public ObjectRemoveComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ObjectRemoveMessageComposer)
        {
            base.WriteString(Item.Id.ToString());
            base.WriteBoolean(false);
            base.WriteInteger(UserId);
            base.WriteInteger(0);
        }
    }
}