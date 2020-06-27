namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class LTDSoldAlertComposer : ServerPacket
    {
        public LTDSoldAlertComposer()
            : base(ServerPacketHeader.LTDSoldAlertComposer)
        {
            base.WriteInteger(0);
        }
    }
}
