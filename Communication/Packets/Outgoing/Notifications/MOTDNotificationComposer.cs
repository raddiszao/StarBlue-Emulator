namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class MOTDNotificationComposer : MessageComposer
    {
        private string Message { get; }

        public MOTDNotificationComposer(string Message)
            : base(Composers.MOTDNotificationMessageComposer)
        {
            this.Message = Message;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteString(Message);
        }
    }
}
