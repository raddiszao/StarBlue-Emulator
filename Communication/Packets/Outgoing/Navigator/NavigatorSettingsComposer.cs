namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorSettingsComposer : MessageComposer
    {
        private int HomeRoom { get; }

        public NavigatorSettingsComposer(int Homeroom)
            : base(Composers.NavigatorSettingsMessageComposer)
        {
            this.HomeRoom = Homeroom;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(HomeRoom);
            packet.WriteInteger(HomeRoom);
        }
    }
}
