namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    class GameUnknownComposer2 : ServerPacket
    {
        public GameUnknownComposer2()
            : base(ServerPacketHeader.GameUnknownComposer1)
        {
        }
    }
}
