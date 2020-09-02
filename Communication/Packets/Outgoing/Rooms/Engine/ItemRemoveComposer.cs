
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemRemoveComposer : ServerPacket
    {
        public ItemRemoveComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ItemRemoveMessageComposer)
        {
            base.WriteString(Item.Id.ToString());
            base.WriteBoolean(false);
            base.WriteInteger(UserId);
        }
    }
}
