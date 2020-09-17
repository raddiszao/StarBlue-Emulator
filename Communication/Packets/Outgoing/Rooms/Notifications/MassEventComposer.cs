namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class MassEventComposer : MessageComposer
    {
        private string Message { get; }

        public MassEventComposer(string Message)
            : base(Composers.MassEventComposer)
        {
            this.Message = Message;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Message);
        }
    }
}