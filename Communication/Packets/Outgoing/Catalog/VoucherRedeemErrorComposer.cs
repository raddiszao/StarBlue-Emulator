namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemErrorComposer : MessageComposer
    {
        private int Type { get; }

        public VoucherRedeemErrorComposer(int Type)
            : base(Composers.VoucherRedeemErrorMessageComposer)
        {
            this.Type = Type;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Type.ToString());
        }
    }
}