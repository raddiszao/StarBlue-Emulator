namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class RefreshFavouriteGroupComposer : ServerPacket
    {
        public RefreshFavouriteGroupComposer(int Id)
            : base(ServerPacketHeader.RefreshFavouriteGroupMessageComposer)
        {
            base.WriteInteger(Id);
        }
    }
}
