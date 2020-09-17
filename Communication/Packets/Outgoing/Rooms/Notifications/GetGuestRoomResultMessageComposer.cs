namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class GetGuestRoomResultMessageComposer : MessageComposer
    {
        private int RoomId { get; }

        public GetGuestRoomResultMessageComposer(int roomId)
            : base(Composers.GetGuestRoomResultMessageComposer)
        {
            this.RoomId = roomId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}
