namespace StarBlue.Communication.Packets.Outgoing.Help
{
    internal class SanctionStatusComposer : MessageComposer
    {
        public SanctionStatusComposer()
            : base(Composers.SanctionStatusMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(true);
            packet.WriteBoolean(false);
            packet.WriteString("ALERT");
            packet.WriteInteger(0);
            packet.WriteInteger(30);
            packet.WriteString("cfh.reason.EMPTY");
            packet.WriteString("2016-07-17 16:33 (GMT +0000)");
            packet.WriteInteger(720);
            packet.WriteString("ALERT");
            packet.WriteInteger(0);
            packet.WriteInteger(30);
            packet.WriteString("");
            packet.WriteBoolean(false);
        }
    }
}