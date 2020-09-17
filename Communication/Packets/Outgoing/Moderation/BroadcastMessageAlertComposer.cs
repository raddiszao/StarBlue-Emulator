namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class BroadcastMessageAlertComposer : MessageComposer
    {
        private string Message { get; }
        private string URL { get; }

        public BroadcastMessageAlertComposer(string Message, string URL = "")
            : base(Composers.BroadcastMessageAlertMessageComposer)
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

