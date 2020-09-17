namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class SendHotelAlertLinkEventComposer : MessageComposer
    {
        private string Message { get; }
        private string URL { get; }

        public SendHotelAlertLinkEventComposer(string Message, string URL = "")
            : base(Composers.SendHotelAlertLinkEvent)
        {
            this.Message = Message;
            this.URL = URL;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Message);
            packet.WriteString(URL);
        }
    }
}
