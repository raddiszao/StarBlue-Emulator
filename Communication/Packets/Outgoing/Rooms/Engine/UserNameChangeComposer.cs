namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UserNameChangeComposer : MessageComposer
    {
        public int RoomId { get; }
        public int VirtualId { get; }
        public string Username { get; }

        public UserNameChangeComposer(int RoomId, int VirtualId, string Username)
            : base(Composers.UserNameChangeMessageComposer)
        {
            this.RoomId = RoomId;
            this.VirtualId = VirtualId;
            this.Username = Username;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(VirtualId);
            packet.WriteString(Username);
        }
    }
}
