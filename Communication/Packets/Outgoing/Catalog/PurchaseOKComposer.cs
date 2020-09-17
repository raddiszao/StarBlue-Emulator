using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class PurchaseOKComposer : MessageComposer
    {
        private CatalogItem Item { get; }
        private ItemData BaseItem { get; }

        public PurchaseOKComposer(CatalogItem Item, ItemData BaseItem)
            : base(Composers.PurchaseOKMessageComposer)
        {
            this.Item = Item;
            this.BaseItem = BaseItem;
        }

        public override void Compose(Composer packet)
        {
            if (this.Item == null)
            {
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteBoolean(false);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteInteger(0);
                packet.WriteBoolean(true);
                packet.WriteInteger(1);
                packet.WriteString("s");
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteInteger(1);
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteInteger(1);
            }
            else
            {
                packet.WriteInteger(BaseItem.Id);
                packet.WriteString(BaseItem.ItemName);
                packet.WriteBoolean(false);
                packet.WriteInteger(Item.CostCredits);
                packet.WriteInteger(Item.CostPixels);
                packet.WriteInteger(0);
                packet.WriteBoolean(true);
                packet.WriteInteger(1);
                packet.WriteString(BaseItem.Type.ToString().ToLower());
                packet.WriteInteger(BaseItem.SpriteId);
                packet.WriteString("");
                packet.WriteInteger(1);
                packet.WriteInteger(0);
                packet.WriteString("");
                packet.WriteInteger(1);
            }
        }

        public PurchaseOKComposer()
            : base(Composers.PurchaseOKMessageComposer)
        {
            this.Item = null;
            this.BaseItem = null;
        }
    }
}