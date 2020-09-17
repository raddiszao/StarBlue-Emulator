namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomEntryInfoComposer : MessageComposer
    {
        public int RoomId { get; }
        public bool IsOwner { get; }

        public RoomEntryInfoComposer(int roomID, bool isOwner)
            : base(Composers.RoomEntryInfoMessageComposer)
        {
            this.RoomId = roomID;
            this.IsOwner = isOwner;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteBoolean(IsOwner);
        }
    }
}
