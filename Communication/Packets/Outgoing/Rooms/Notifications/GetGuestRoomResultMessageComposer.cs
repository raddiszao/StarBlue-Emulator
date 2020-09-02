namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class GetGuestRoomResultMessageComposer : ServerPacket
    {
        public GetGuestRoomResultMessageComposer(int roomId)
            : base(ServerPacketHeader.GetGuestRoomResultMessageComposer)
        {
            base.WriteInteger(roomId);
        }
    }
}
