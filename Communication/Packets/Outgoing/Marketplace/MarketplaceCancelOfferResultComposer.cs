namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceCancelOfferResultComposer : MessageComposer
    {
        public int OfferId { get; }
        public bool Success { get; }

        public MarketplaceCancelOfferResultComposer(int OfferId, bool Success)
            : base(Composers.MarketplaceCancelOfferResultMessageComposer)
        {
            this.OfferId = OfferId;
            this.Success = Success;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(OfferId);
            packet.WriteBoolean(Success);
        }
    }
}
