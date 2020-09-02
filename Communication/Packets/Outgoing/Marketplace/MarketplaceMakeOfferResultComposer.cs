namespace StarBlue.Communication.Packets.Outgoing.Marketplace
{
    internal class MarketplaceMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceMakeOfferResultComposer(int Success)
            : base(ServerPacketHeader.MarketplaceMakeOfferResultMessageComposer)
        {
            base.WriteInteger(Success);
        }
    }
}
