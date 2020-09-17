namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingCompleteComposer : MessageComposer
    {
        public TradingCompleteComposer()
            : base(Composers.TradingCompleteMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {

        }
    }
}
