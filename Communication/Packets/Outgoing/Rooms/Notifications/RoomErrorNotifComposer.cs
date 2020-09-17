namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class RoomErrorNotifComposer : MessageComposer
    {
        public int Error { get; }
        public RoomErrorNotifComposer(int Error)
            : base(Composers.RoomErrorNotifMessageComposer)
        {
            this.Error = Error;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Error);
        }
    }
}
