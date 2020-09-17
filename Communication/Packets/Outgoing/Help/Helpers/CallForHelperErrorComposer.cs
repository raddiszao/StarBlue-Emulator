namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class CallForHelperErrorComposer : MessageComposer
    {
        private int errorCode { get; }

        public CallForHelperErrorComposer(int errorCode)
            : base(Composers.CallForHelperErrorMessageComposer)
        {
            this.errorCode = errorCode;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(errorCode);
        }
    }
}
