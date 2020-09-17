namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class CloseHelperSessionComposer : MessageComposer
    {
        public CloseHelperSessionComposer()
            : base(Composers.CloseHelperSessionMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
        }
    }
}
