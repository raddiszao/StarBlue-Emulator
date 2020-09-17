namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class FlatControllerRemovedComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }
        public FlatControllerRemovedComposer(int RoomId, int UserId)
            : base(Composers.FlatControllerRemovedMessageComposer)
        {
            this.RoomId = RoomId;
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(UserId);
        }
    }
}
