namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    class MaximizedTargettedOfferComposer : ServerPacket
    {
        public MaximizedTargettedOfferComposer()
            : base(ServerPacketHeader.MaximizedTargettedOfferComposer)
        {
            base.WriteInteger(1);
            base.WriteInteger(1);
        }
    }
}