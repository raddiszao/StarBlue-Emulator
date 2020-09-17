namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class LTDSoldAlertComposer : MessageComposer
    {
        public LTDSoldAlertComposer()
            : base(Composers.LTDSoldAlertComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
        }
    }
}
