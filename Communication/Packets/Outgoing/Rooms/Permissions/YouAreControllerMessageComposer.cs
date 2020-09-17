namespace StarBlue.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreControllerComposer : MessageComposer
    {
        private int Setting { get; }

        public YouAreControllerComposer(int Setting)
            : base(Composers.YouAreControllerMessageComposer)
        {
            this.Setting = Setting;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Setting);
        }
    }
}
