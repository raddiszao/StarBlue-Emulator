namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    internal class OpenConnectionComposer : MessageComposer
    {
        public OpenConnectionComposer()
            : base(Composers.OpenConnectionMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
        }
    }
}
