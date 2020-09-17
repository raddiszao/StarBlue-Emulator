namespace StarBlue.Communication.Packets.Outgoing.SMS
{
    internal class SMSErrorComposer : MessageComposer
    {
        public SMSErrorComposer()
            : base(Composers.SMSErrorComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(2);
            packet.WriteInteger(2);
        }
    }
}
