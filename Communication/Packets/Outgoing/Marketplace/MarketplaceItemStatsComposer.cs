namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceItemStatsComposer : MessageComposer
    {
        public int ItemId { get; }
        public int SpriteId { get; }
        public int AveragePrice { get; }

        public MarketplaceItemStatsComposer(int ItemId, int SpriteId, int AveragePrice)
            : base(Composers.MarketplaceItemStatsMessageComposer)
        {
            this.ItemId = ItemId;
            this.SpriteId = SpriteId;
            this.AveragePrice = AveragePrice;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(AveragePrice);//Avg price in last 7 days.
            packet.WriteInteger(StarBlueServer.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(SpriteId));

            packet.WriteInteger(0);//No idea.
            packet.WriteInteger(0);//No idea.

            packet.WriteInteger(ItemId);
            packet.WriteInteger(SpriteId);
        }
    }
}