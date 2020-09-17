namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingClosedComposer : MessageComposer
    {
        public int UserId { get; }

        public TradingClosedComposer(int UserId)
            : base(Composers.TradingClosedMessageComposer)
        {
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(0);
        }
    }
}
