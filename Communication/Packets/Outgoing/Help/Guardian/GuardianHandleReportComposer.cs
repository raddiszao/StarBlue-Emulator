namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class GuardianHandleReportComposer : MessageComposer
    {
        private int seconds { get; }

        public GuardianHandleReportComposer(int seconds)
            : base(Composers.GuardianHandleReportMessageComposer)
        {
            this.seconds = seconds;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(seconds);
        }
    }
}
