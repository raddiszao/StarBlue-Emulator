namespace StarBlue.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreNotControllerComposer : MessageComposer
    {
        public YouAreNotControllerComposer()
            : base(Composers.YouAreNotControllerMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
        }
    }
}
