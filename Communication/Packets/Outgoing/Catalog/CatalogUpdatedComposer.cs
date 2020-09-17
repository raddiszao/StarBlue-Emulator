namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogUpdatedComposer : MessageComposer
    {
        public CatalogUpdatedComposer()
            : base(Composers.CatalogUpdatedMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(false);
        }
    }
}
