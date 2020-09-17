namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    internal class RoomReadyComposer : MessageComposer
    {
        private int RoomId { get; }
        private string Model { get; }

        public RoomReadyComposer(int RoomId, string Model)
            : base(Composers.RoomReadyMessageComposer)
        {
            this.RoomId = RoomId;
            this.Model = Model;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(Model);
            packet.WriteInteger(RoomId);
        }
    }
}
