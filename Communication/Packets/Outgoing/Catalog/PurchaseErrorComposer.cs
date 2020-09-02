namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class PurchaseErrorComposer : ServerPacket
    {
        public PurchaseErrorComposer(int ErrorCode)
            : base(ServerPacketHeader.PurchaseErrorMessageComposer)
        {
            base.WriteInteger(ErrorCode);
        }
    }
}
