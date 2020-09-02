namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomSpectatorComposer : ServerPacket
    {
        public RoomSpectatorComposer()
            : base(ServerPacketHeader.RoomSpectatorComposer)
        {
        }
    }
}
