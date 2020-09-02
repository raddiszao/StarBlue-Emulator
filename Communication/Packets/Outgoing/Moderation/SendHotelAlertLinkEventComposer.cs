namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class SendHotelAlertLinkEventComposer : ServerPacket
    {
        public SendHotelAlertLinkEventComposer(string Message, string URL = "")
            : base(ServerPacketHeader.SendHotelAlertLinkEvent)
        {
            base.WriteString(Message);
            base.WriteString(URL);
        }
    }
}
