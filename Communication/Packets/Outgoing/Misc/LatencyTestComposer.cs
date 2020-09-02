namespace StarBlue.Communication.Packets.Outgoing.Misc
{
    internal class LatencyTestComposer : ServerPacket
    {
        public LatencyTestComposer(int response)
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {
            base.WriteInteger(response);
        }
    }
}
