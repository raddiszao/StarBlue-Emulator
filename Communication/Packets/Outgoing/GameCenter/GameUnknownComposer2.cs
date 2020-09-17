namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    internal class GameUnknownComposer2 : MessageComposer
    {
        public GameUnknownComposer2()
            : base(Composers.GameUnknownComposer1)
        {
        }

        public override void Compose(Composer packet)
        {
        }
    }
}
