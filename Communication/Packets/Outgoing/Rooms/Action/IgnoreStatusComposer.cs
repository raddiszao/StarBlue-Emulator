namespace StarBlue.Communication.Packets.Outgoing.Rooms.Action
{
    internal class IgnoreStatusComposer : MessageComposer
    {
        private int Status { get; }
        private string Username { get; }

        public IgnoreStatusComposer(int Status, string Username)
            : base(Composers.IgnoreStatusMessageComposer)
        {
            this.Status = Status;
            this.Username = Username;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Status);
            packet.WriteString(Username);
        }
    }
}
