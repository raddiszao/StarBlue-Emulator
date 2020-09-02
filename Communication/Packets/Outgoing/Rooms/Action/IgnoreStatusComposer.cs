namespace StarBlue.Communication.Packets.Outgoing.Rooms.Action
{
    internal class IgnoreStatusComposer : ServerPacket
    {
        public IgnoreStatusComposer(int Status, string Username)
            : base(ServerPacketHeader.IgnoreStatusMessageComposer)
        {
            base.WriteInteger(Status);
            base.WriteString(Username);
        }
    }
}
