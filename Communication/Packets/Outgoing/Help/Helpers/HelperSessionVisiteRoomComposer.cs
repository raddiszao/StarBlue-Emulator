namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class HelperSessionVisiteRoomComposer : ServerPacket
    {
        public HelperSessionVisiteRoomComposer(int roomId)
            : base(ServerPacketHeader.HelperSessionVisiteRoomMessageComposer)
        {
            base.WriteInteger(roomId);
        }
    }
}
