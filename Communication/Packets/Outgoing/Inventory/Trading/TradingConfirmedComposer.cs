namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingConfirmedComposer : MessageComposer
    {
        public int UserId { get; }
        public bool Confirmed { get; }

        public TradingConfirmedComposer(int UserId, bool Confirmed)
            : base(Composers.TradingConfirmedMessageComposer)
        {
            this.UserId = UserId;
            this.Confirmed = Confirmed;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(Confirmed ? 1 : 0);
        }
    }
}
