namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceCanMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceCanMakeOfferResultComposer(int Result)
            : base(ServerPacketHeader.MarketplaceCanMakeOfferResultMessageComposer)
        {
            base.WriteInteger(Result);
            base.WriteInteger(0);
        }
    }
}
