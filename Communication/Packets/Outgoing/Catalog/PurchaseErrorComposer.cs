namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class PurchaseErrorComposer : MessageComposer
    {
        private int ErrorCode { get; }

        public PurchaseErrorComposer(int ErrorCode)
            : base(Composers.PurchaseErrorMessageComposer)
        {
            this.ErrorCode = ErrorCode;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ErrorCode);
        }
    }
}
