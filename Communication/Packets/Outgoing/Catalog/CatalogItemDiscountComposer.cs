namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class CatalogItemDiscountComposer : MessageComposer
    {
        public CatalogItemDiscountComposer()
            : base(Composers.CatalogItemDiscountMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(100);//Most you can get.
            packet.WriteInteger(6);
            packet.WriteInteger(1);
            packet.WriteInteger(1);
            packet.WriteInteger(2);//Count
            {
                packet.WriteInteger(40);
                packet.WriteInteger(99);
            }
        }
    }
}