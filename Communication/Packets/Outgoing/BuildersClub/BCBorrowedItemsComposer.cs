namespace StarBlue.Communication.Packets.Outgoing.BuildersClub
{
    internal class BCBorrowedItemsComposer : ServerPacket
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.BCBorrowedItemsMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
