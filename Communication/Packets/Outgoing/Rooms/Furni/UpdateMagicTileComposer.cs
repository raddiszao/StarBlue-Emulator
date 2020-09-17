namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class UpdateMagicTileComposer : MessageComposer
    {
        public int ItemId { get; }
        public int Decimal { get; }

        public UpdateMagicTileComposer(int ItemId, int Decimal)
            : base(Composers.UpdateMagicTileMessageComposer)
        {
            this.ItemId = ItemId;
            this.Decimal = Decimal;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteInteger(Decimal);
        }
    }
}
