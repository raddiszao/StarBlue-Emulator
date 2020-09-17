
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemRemoveComposer : MessageComposer
    {
        public int ItemId { get; }
        public int UserId { get; }

        public ItemRemoveComposer(Item Item, int UserId)
            : base(Composers.ItemRemoveMessageComposer)
        {
            this.ItemId = Item.Id;
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(ItemId.ToString());
            packet.WriteBoolean(false);
            packet.WriteInteger(UserId);
        }
    }
}
