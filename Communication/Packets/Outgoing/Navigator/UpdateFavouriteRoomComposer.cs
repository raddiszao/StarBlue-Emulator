namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    public class UpdateFavouriteRoomComposer : MessageComposer
    {
        private int RoomId { get; }
        private bool Added { get; }

        public UpdateFavouriteRoomComposer(int RoomId, bool Added)
            : base(Composers.UpdateFavouriteRoomMessageComposer)
        {
            this.RoomId = RoomId;
            this.Added = Added;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteBoolean(Added);
        }
    }
}