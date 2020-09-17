namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingFinishComposer : MessageComposer
    {
        public TradingFinishComposer()
            : base(Composers.TradingFinishMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {

        }
    }
}
