namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemOkComposer : MessageComposer
    {
        public VoucherRedeemOkComposer()
            : base(Composers.VoucherRedeemOkMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("");//productName
            packet.WriteString("");//productDescription
        }
    }
}