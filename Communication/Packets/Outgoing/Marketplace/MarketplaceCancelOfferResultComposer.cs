namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceCancelOfferResultComposer : ServerPacket
    {
        public MarketplaceCancelOfferResultComposer(int OfferId, bool Success)
            : base(ServerPacketHeader.MarketplaceCancelOfferResultMessageComposer)
        {
            base.WriteInteger(OfferId);
            base.WriteBoolean(Success);
        }
    }
}
