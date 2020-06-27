namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    class MysticBoxCloseComposer : ServerPacket
    {
        public MysticBoxCloseComposer()
            : base(ServerPacketHeader.MysticBoxCloseComposer)
        {
        }
    }
}