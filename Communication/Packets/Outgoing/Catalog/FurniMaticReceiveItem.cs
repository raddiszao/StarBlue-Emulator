namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class FurniMaticReceiveItem : MessageComposer
    {
        private int GiveItemId { get; }

        public FurniMaticReceiveItem(int Id)
            : base(Composers.FurniMaticReceiveItem)
        {
            this.GiveItemId = Id;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(GiveItemId); // received item id
        }
    }
}
