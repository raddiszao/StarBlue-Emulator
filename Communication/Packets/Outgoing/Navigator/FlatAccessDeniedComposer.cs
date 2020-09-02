namespace StarBlue.Communication.Packets.Outgoing.Navigator
{
    internal class FlatAccessDeniedComposer : ServerPacket
    {
        public FlatAccessDeniedComposer(string Username)
            : base(ServerPacketHeader.FlatAccessDeniedMessageComposer)
        {
            base.WriteString(Username);
        }
    }
}
