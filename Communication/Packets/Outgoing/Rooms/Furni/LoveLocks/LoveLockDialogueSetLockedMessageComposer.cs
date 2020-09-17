namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks
{
    class LoveLockDialogueSetLockedMessageComposer : MessageComposer
    {
        public int ItemId { get; }

        public LoveLockDialogueSetLockedMessageComposer(int ItemId)
            : base(Composers.LoveLockDialogueSetLockedMessageComposer)
        {
            this.ItemId = ItemId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(ItemId);
        }
    }
}
