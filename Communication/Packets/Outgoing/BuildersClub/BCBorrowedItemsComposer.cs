namespace StarBlue.Communication.Packets.Outgoing.BuildersClub
{
    internal class BCBorrowedItemsComposer : MessageComposer
    {
        public BCBorrowedItemsComposer()
            : base(Composers.BCBorrowedItemsMessageComposer)
        {

        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0);
        }
    }
}
