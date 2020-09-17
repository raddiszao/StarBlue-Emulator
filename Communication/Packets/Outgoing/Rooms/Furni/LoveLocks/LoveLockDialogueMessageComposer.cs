namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueMessageComposer : MessageComposer
    {
        public int ItemId { get; }

        public LoveLockDialogueMessageComposer(int ItemId)
            : base(Composers.LoveLockDialogueMessageComposer)
        {
            this.ItemId = ItemId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ItemId);
            packet.WriteBoolean(true);
        }
    }
}
