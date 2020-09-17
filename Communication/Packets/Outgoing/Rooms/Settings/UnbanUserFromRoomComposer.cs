namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class UnbanUserFromRoomComposer : MessageComposer
    {
        public int RoomId { get; }
        public int UserId { get; }
        public UnbanUserFromRoomComposer(int RoomId, int UserId)
            : base(Composers.UnbanUserFromRoomMessageComposer)
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
