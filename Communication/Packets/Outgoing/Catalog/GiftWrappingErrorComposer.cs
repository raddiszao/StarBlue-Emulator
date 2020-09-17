namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class GiftWrappingErrorComposer : MessageComposer
    {
        public GiftWrappingErrorComposer()
            : base(Composers.GiftWrappingErrorMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
        }
    }
}
