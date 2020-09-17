namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceMakeOfferResultComposer : MessageComposer
    {
        public int Success { get; }

        public MarketplaceMakeOfferResultComposer(int Success)
            : base(Composers.MarketplaceMakeOfferResultMessageComposer)
        {
            this.Success = Success;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Success);
        }
    }
}
