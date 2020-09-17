namespace StarBlue.Communication.Packets.Outgoing.Nux
{
    internal class NuxAlertComposer : MessageComposer
    {
        private string Message { get; }

        public NuxAlertComposer(string Message) : base(Composers.NuxAlertMessageComposer)
        {
            this.Message = Message;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Message);
        }
    }
}
