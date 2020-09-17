namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class MaximizedTargettedOfferComposer : MessageComposer
    {
        public MaximizedTargettedOfferComposer()
            : base(Composers.MaximizedTargettedOfferComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(1);
        }
    }
}