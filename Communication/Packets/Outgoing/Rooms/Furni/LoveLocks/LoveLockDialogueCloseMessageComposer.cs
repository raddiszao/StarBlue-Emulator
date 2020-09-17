namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueCloseMessageComposer : MessageComposer
    {
        public int ItemId { get; }

        public LoveLockDialogueCloseMessageComposer(int ItemId)
            : base(Composers.LoveLockDialogueCloseMessageComposer)
        {
            this.ItemId = ItemId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}
