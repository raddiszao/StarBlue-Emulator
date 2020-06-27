namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    class HelperSessionVisiteRoomComposer : ServerPacket
    {
        public HelperSessionVisiteRoomComposer(int roomId)
            : base(ServerPacketHeader.HelperSessionVisiteRoomMessageComposer)
        {
            base.WriteInteger(roomId);
        }
    }
}
