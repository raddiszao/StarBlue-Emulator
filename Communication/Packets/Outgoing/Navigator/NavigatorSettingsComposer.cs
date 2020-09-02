namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int Homeroom)
            : base(ServerPacketHeader.NavigatorSettingsMessageComposer)
        {
            base.WriteInteger(Homeroom);
            base.WriteInteger(Homeroom);
        }
    }
}
