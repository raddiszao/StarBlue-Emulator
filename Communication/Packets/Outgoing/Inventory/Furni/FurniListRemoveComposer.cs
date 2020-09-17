namespace StarBlue.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListRemoveComposer : MessageComposer
    {
        public int FurniId { get; }

        public FurniListRemoveComposer(int Id)
            : base(Composers.FurniListRemoveMessageComposer)
        {
            this.FurniId = Id;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(FurniId);
        }
    }
}
