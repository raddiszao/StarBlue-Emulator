namespace StarBlue.Communication.Packets.Outgoing.SMS
{
    class SMSVerifyComposer : ServerPacket
    {
        public SMSVerifyComposer(int value1, int value2)
            : base(ServerPacketHeader.SMSVerifyComposer)
        {
            base.WriteInteger(value1);
            base.WriteInteger(value2);
        }
    }
}
