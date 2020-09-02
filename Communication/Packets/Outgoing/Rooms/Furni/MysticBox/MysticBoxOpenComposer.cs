namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class MysticBoxOpenComposer : ServerPacket
    {
        public MysticBoxOpenComposer()
            : base(ServerPacketHeader.MysticBoxOpenComposer)
        {
        }
    }
}