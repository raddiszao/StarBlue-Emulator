namespace StarBlue.Communication.Packets.Outgoing.Rooms.Permissions
{
    internal class YouAreNotControllerComposer : ServerPacket
    {
        public YouAreNotControllerComposer()
            : base(ServerPacketHeader.YouAreNotControllerMessageComposer)
        {
        }
    }
}
