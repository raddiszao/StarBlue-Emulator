namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorPreferencesComposer : MessageComposer
    {
        public NavigatorPreferencesComposer()
            : base(Composers.NavigatorPreferencesMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(95);
            packet.WriteInteger(60);
            packet.WriteInteger(425);
            packet.WriteInteger(450);
            packet.WriteBoolean(false);
            packet.WriteInteger(0);
        }
    }
}