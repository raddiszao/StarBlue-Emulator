namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class WiredSmartAlertComposer : MessageComposer
    {
        private string Message { get; }

        public WiredSmartAlertComposer(string Message)
            : base(Composers.WiredSmartAlertComposer)
        {
            this.Message = Message;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Message);
        }
    }
}