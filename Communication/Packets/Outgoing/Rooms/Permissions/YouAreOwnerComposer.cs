namespace StarBlue.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreOwnerComposer : ServerPacket
    {
        public YouAreOwnerComposer()
            : base(ServerPacketHeader.YouAreOwnerMessageComposer)
        {
        }
    }
}
