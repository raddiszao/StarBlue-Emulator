namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class HabboMallOfferComposer : MessageComposer
    {
        public HabboMallOfferComposer()
            : base(Composers.HabboMallOfferComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString("Test");
            packet.WriteString("imagen");
        }
    }
}