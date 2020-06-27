namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    class GetGuestRoomResultMessageComposer : ServerPacket
    {
        public GetGuestRoomResultMessageComposer(int roomId)
            : base(ServerPacketHeader.GetGuestRoomResultMessageComposer)
        {
            base.WriteInteger(roomId);
        }
    }
}
