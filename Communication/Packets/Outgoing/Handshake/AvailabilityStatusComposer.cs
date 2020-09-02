namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class AvailabilityStatusComposer : ServerPacket
    {
        public AvailabilityStatusComposer()
            : base(ServerPacketHeader.AvailabilityStatusMessageComposer)
        {
            base.WriteBoolean(true);
            base.WriteBoolean(false);
            base.WriteBoolean(true);
        }
    }
}
