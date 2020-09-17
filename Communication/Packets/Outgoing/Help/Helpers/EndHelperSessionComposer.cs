namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class EndHelperSessionComposer : MessageComposer
    {
        private int closeCode { get; }

        public EndHelperSessionComposer(int closeCode = 0)
            : base(Composers.EndHelperSessionMessageComposer)
        {
            this.closeCode = closeCode;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(closeCode);
        }
    }
}
