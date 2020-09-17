namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class AvailabilityStatusComposer : MessageComposer
    {
        public AvailabilityStatusComposer()
            : base(Composers.AvailabilityStatusMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(true);
            packet.WriteBoolean(false);
            packet.WriteBoolean(true);
        }
    }
}
