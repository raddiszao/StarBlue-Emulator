namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    class HabboMallOfferComposer : ServerPacket
    {
        public HabboMallOfferComposer()
            : base(ServerPacketHeader.HabboMallOfferComposer)
        {
            base.WriteString("Test");
            base.WriteString("imagen");
        }
    }
}