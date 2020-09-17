namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    internal class CantConnectComposer : MessageComposer
    {
        private int Error { get; }

        public CantConnectComposer(int Error)
            : base(Composers.CantConnectMessageComposer)
        {
            this.Error = Error;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Error);
        }
    }
}
