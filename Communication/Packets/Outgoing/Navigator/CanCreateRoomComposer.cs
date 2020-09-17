namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class CanCreateRoomComposer : MessageComposer
    {
        private bool Error { get; }
        private int MaxRoomsPerUser { get; }

        public CanCreateRoomComposer(bool Error, int MaxRoomsPerUser)
            : base(Composers.CanCreateRoomMessageComposer)
        {
            this.Error = Error;
            this.MaxRoomsPerUser = MaxRoomsPerUser;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Error ? 1 : 0);
            packet.WriteInteger(MaxRoomsPerUser);
        }
    }
}
