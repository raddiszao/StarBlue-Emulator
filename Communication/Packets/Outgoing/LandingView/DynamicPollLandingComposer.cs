namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    internal class DynamicPollLandingComposer : ServerPacket
    {
        public DynamicPollLandingComposer(bool HasDone)
            : base(ServerPacketHeader.DynamicPollLandingComposer)
        {
            base.WriteBoolean(HasDone);
        }
    }
}
