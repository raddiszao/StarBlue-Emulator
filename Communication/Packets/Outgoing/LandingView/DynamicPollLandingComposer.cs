namespace StarBlue.Communication.Packets.Outgoing.LandingView
{
    internal class DynamicPollLandingComposer : MessageComposer
    {
        private bool HasDone { get; }

        public DynamicPollLandingComposer(bool HasDone)
            : base(Composers.DynamicPollLandingComposer)
        {
            this.HasDone = HasDone;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(HasDone);
        }
    }
}
