namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerAddedComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }
        public string Username { get; }
        public FlatControllerAddedComposer(int RoomId, int UserId, string Username)
            : base(Composers.FlatControllerAddedMessageComposer)
        {
            this.RoomId = RoomId;
            this.UserId = UserId;
            this.Username = Username;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(UserId);
            packet.WriteString(Username);
        }
    }
}
