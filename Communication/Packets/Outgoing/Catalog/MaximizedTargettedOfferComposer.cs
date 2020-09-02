namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class MaximizedTargettedOfferComposer : ServerPacket
    {
        public MaximizedTargettedOfferComposer()
            : base(ServerPacketHeader.MaximizedTargettedOfferComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(1);
        }
    }
}