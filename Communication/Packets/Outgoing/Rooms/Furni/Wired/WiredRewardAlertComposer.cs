namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    public class WiredRewardAlertComposer : MessageComposer
    {
        private int codeMsg { get; }

        public WiredRewardAlertComposer(int codeMsg)
            : base(Composers.WiredRewardAlertComposer)
        {
            this.codeMsg = codeMsg;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(codeMsg);
        }
    }
}
