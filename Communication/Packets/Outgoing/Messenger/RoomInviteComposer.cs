namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class RoomInviteComposer : ServerPacket
    {
        public RoomInviteComposer(int SenderId, string Text)
            : base(ServerPacketHeader.RoomInviteMessageComposer)
        {
            base.WriteInteger(SenderId);
            base.WriteString(Text);
        }
    }
}
