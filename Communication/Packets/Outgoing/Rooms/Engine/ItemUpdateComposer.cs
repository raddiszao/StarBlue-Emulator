
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemUpdateComposer : MessageComposer
    {
        public Item Item { get; }
        public int UserId { get; }

        public ItemUpdateComposer(Item Item, int UserId)
            : base(Composers.ItemUpdateMessageComposer)
        {
            this.Item = Item;
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            WriteWallItem(Item, UserId, packet);
        }

        private void WriteWallItem(Item Item, int UserId, Composer packet)
        {
            packet.WriteString(Item.Id.ToString());
            packet.WriteInteger(Item.GetBaseItem().SpriteId);
            packet.WriteString(Item.wallCoord);
            switch (Item.GetBaseItem().InteractionType)
            {
                case InteractionType.POSTIT:
                    packet.WriteString(Item.ExtraData.Split(' ')[0]);
                    break;

                default:
                    packet.WriteString(Item.ExtraData);
                    break;
            }
            packet.WriteInteger(-1);
            packet.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
            packet.WriteInteger(UserId);
        }
    }
}