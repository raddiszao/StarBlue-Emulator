namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class NewYearResolutionCompletedComposer : ServerPacket
    {
        public NewYearResolutionCompletedComposer(string badge)
            : base(ServerPacketHeader.NewYearResolutionCompletedComposer)
        {
            base.WriteString(badge);
            base.WriteString(badge);
        }
    }
}

