namespace StarBlue.Communication.Packets.Outgoing.SMS
{
    class SMSErrorComposer : ServerPacket
    {
        public SMSErrorComposer()
            : base(ServerPacketHeader.SMSErrorComposer)
        {
            base.WriteInteger(2);
            base.WriteInteger(2);
        }
    }
}
