namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogUpdatedComposer : ServerPacket
    {
        public CatalogUpdatedComposer()
            : base(ServerPacketHeader.CatalogUpdatedMessageComposer)
        {
            base.WriteBoolean(false);
        }
    }
}
