namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class GnomeBoxComposer : MessageComposer
    {
        public int ItemId { get; }

        public GnomeBoxComposer(int ItemId)
            : base(Composers.GnomeBoxMessageComposer)
        {
            this.ItemId = ItemId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}
