namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceCanMakeOfferResultComposer : MessageComposer
    {
        public int Result { get; }

        public MarketplaceCanMakeOfferResultComposer(int Result)
            : base(Composers.MarketplaceCanMakeOfferResultMessageComposer)
        {
            this.Result = Result;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Result);
            packet.WriteInteger(0);
        }
    }
}
