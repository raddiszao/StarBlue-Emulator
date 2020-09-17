namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class RoomCustomizedAlertComposer : MessageComposer
    {
        private string Message { get; }

        public RoomCustomizedAlertComposer(string Message)
            : base(Composers.RoomCustomizedAlertComposer)

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