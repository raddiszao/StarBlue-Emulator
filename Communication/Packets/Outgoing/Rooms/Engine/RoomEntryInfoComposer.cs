namespace StarBlue.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class RoomEntryInfoComposer : ServerPacket
    {
        public RoomEntryInfoComposer(int roomID, bool isOwner)
            : base(ServerPacketHeader.RoomEntryInfoMessageComposer)
        {
            base.WriteInteger(roomID);
            base.WriteBoolean(isOwner);
        }
    }
}
