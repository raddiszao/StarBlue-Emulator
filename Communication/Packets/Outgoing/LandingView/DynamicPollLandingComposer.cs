namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    class DynamicPollLandingComposer : ServerPacket
    {
        public DynamicPollLandingComposer(bool HasDone)
            : base(ServerPacketHeader.DynamicPollLandingComposer)
        {
            base.WriteBoolean(HasDone);
        }
    }
}
