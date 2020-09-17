namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    public class RoomForwardComposer : MessageComposer
    {
        private int RoomId { get; }

        public RoomForwardComposer(int RoomId)
            : base(Composers.RoomForwardMessageComposer)
        {
            this.RoomId = RoomId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
        }
    }
}