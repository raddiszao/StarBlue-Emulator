namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingAcceptComposer : MessageComposer
    {
        public int UserId { get; }
        public bool Accept { get; }

        public TradingAcceptComposer(int UserId, bool Accept)
            : base(Composers.TradingAcceptMessageComposer)
        {
            this.UserId = UserId;
            this.Accept = Accept;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(Accept ? 1 : 0);
        }
    }
}
