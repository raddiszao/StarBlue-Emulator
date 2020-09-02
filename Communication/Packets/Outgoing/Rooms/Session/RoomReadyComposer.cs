namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    internal class RoomReadyComposer : ServerPacket
    {
        public RoomReadyComposer(int RoomId, string Model)
            : base(ServerPacketHeader.RoomReadyMessageComposer)
        {
            base.WriteString(Model);
            base.WriteInteger(RoomId);
        }
    }
}
