
using StarBlue.HabboHotel.Catalog;
using StarBlue.HabboHotel.Catalog.Utilities;
using StarBlue.HabboHotel.Items;

namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogOfferComposer : MessageComposer
    {
        private CatalogItem Item { get; }

        public CatalogOfferComposer(CatalogItem Item)
            : base(Composers.CatalogOfferMessageComposer)
        {
            this.Item = Item;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Item.OfferId);
            packet.WriteString(Item.Data.ItemName);
            packet.WriteBoolean(false);//IsRentable
            packet.WriteInteger(Item.CostCredits);

            if (Item.CostDiamonds > 0)
            {
                packet.WriteInteger(Item.CostDiamonds);
                packet.WriteInteger(5); // Diamonds
            }
            else if (Item.CostGOTWPoints > 0)
            {
                packet.WriteInteger(Item.CostGOTWPoints);
                packet.WriteInteger(103); // Puntos de Honor
            }
            else
            {
                packet.WriteInteger(Item.CostPixels);
                packet.WriteInteger(0); // Type of PixelCost
            }

            packet.WriteBoolean(ItemUtility.CanGiftItem(Item));
            packet.WriteInteger(string.IsNullOrEmpty(Item.Badge) ? 1 : 2);//Count 1 item if there is no badge, otherwise count as 2.

            if (!string.IsNullOrEmpty(Item.Badge))
            {
                packet.WriteString("b");
                packet.WriteString(Item.Badge);
            }

            packet.WriteString(Item.Data.Type.ToString());
            if (Item.Data.Type.ToString().ToLower() == "b")
            {
                packet.WriteString(Item.Data.ItemName);//Badge name.
            }
            else
            {
                packet.WriteInteger(Item.Data.SpriteId);
                if (Item.Data.InteractionType == InteractionType.WALLPAPER || Item.Data.InteractionType == InteractionType.FLOOR || Item.Data.InteractionType == InteractionType.LANDSCAPE)
                {
                    packet.WriteString(Item.Name.Split('_')[2]);
                }
                else if (Item.PageID == 9)//Bots
                {
                    if (!StarBlueServer.GetGame().GetCatalog().TryGetBot(Item.ItemId, out CatalogBot CataBot))
                    {
                        packet.WriteString("hd-180-7.ea-1406-62.ch-210-1321.hr-831-49.ca-1813-62.sh-295-1321.lg-285-92");
                    }
                    else
                    {
                        packet.WriteString(CataBot.Figure);
                    }
                }
                else if (Item.ExtraData != null)
                {
                    packet.WriteString(Item.ExtraData != null ? Item.ExtraData : string.Empty);
                }

                packet.WriteInteger(Item.Amount);
                packet.WriteBoolean(Item.IsLimited); // IsLimited
                if (Item.IsLimited)
                {
                    packet.WriteInteger(Item.LimitedEditionStack);
                    packet.WriteInteger(Item.LimitedEditionStack - Item.LimitedEditionSells);
                }
            }
            packet.WriteInteger(0); // club_level
            packet.WriteBoolean(ItemUtility.CanSelectAmount(Item));
            packet.WriteBoolean(true); // Niu Rilí
            packet.WriteString(""); // Niu Rilí
        }
    }
}
