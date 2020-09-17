using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class ItemAddComposer : MessageComposer
    {
        public Item Item { get; }

        public ItemAddComposer(Item Item)
            : base(Composers.ItemAddMessageComposer)
        {
            this.Item = Item;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Item.Id.ToString());
            packet.WriteInteger(Item.GetBaseItem().SpriteId);
            packet.WriteString(Item.wallCoord != null ? Item.wallCoord : string.Empty);

            ItemBehaviourUtility.GenerateWallExtradata(Item, packet);

            packet.WriteInteger(-1);
            packet.WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0); // Type New R63 ('use bottom')
            packet.WriteInteger(Item.UserID);
            packet.WriteString(Item.Username);
        }
    }
}