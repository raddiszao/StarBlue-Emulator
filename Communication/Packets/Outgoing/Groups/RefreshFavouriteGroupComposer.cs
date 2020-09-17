namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class RefreshFavouriteGroupComposer : MessageComposer
    {
        public int GroupId { get; }

        public RefreshFavouriteGroupComposer(int Id)
            : base(Composers.RefreshFavouriteGroupMessageComposer)
        {
            this.GroupId = Id;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GroupId);
        }
    }
}
