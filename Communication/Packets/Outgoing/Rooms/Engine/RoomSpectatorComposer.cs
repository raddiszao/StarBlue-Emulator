namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomSpectatorComposer : MessageComposer
    {
        public RoomSpectatorComposer()
            : base(Composers.RoomSpectatorComposer)
        {
        }

        public override void Compose(Composer packet)
        {
        }
    }
}
