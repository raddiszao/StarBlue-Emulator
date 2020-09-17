namespace StarBlue.Communication.Packets.Outgoing.SMS
{
    internal class SMSVerifyComposer : MessageComposer
    {
        private int value1 { get; }
        private int value2 { get; }

        public SMSVerifyComposer(int value1, int value2)
            : base(Composers.SMSVerifyComposer)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(value1);
            packet.WriteInteger(value2);
        }
    }
}
