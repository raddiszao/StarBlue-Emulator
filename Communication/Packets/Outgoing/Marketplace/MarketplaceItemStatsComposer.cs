namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceItemStatsComposer : ServerPacket
    {
        public MarketplaceItemStatsComposer(int ItemId, int SpriteId, int AveragePrice)
            : base(ServerPacketHeader.MarketplaceItemStatsMessageComposer)
        {
            base.WriteInteger(AveragePrice);//Avg price in last 7 days.
            base.WriteInteger(StarBlueServer.GetGame().GetCatalog().GetMarketplace().OfferCountForSprite(SpriteId));

            base.WriteInteger(0);//No idea.
            base.WriteInteger(0);//No idea.

            base.WriteInteger(ItemId);
            base.WriteInteger(SpriteId);
        }
    }
}