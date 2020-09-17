namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Stickys
{
    internal class StickyNoteComposer : MessageComposer
    {
        public string ItemId { get; }
        public string ExtraData { get; }

        public StickyNoteComposer(string ItemId, string Extradata)
            : base(Composers.StickyNoteMessageComposer)
        {
            this.ItemId = ItemId;
            this.ExtraData = Extradata;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(ItemId);
            packet.WriteString(ExtraData);
        }
    }
}
