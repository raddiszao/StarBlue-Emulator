namespace StarBlue.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreOwnerComposer : MessageComposer
    {
        public YouAreOwnerComposer()
            : base(Composers.YouAreOwnerMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
        }
    }
}
