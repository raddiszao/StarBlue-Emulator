namespace StarBlue.Communication.Packets.Outgoing.Help
{
    internal class SubmitBullyReportComposer : MessageComposer
    {
        public int Result { get; }

        public SubmitBullyReportComposer(int Result)
            : base(Composers.SubmitBullyReportMessageComposer)
        {
            this.Result = Result;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Result);
        }
    }
}
