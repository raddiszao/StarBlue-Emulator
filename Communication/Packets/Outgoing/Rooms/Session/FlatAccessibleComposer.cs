namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    internal class FlatAccessibleComposer : ServerPacket
    {
        public FlatAccessibleComposer(string Username)
            : base(ServerPacketHeader.FlatAccessibleMessageComposer)
        {
            base.WriteString(Username);
        }
    }
}
