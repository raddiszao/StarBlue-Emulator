namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class RoomInfoUpdatedComposer : MessageComposer
    {
        private int roomID { get; }

        public RoomInfoUpdatedComposer(int roomID)
            : base(Composers.RoomInfoUpdatedMessageComposer)
        {
            this.roomID = roomID;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(roomID);
        }
    }
}
