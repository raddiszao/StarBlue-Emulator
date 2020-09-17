namespace StarBlue.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingErrorComposer : MessageComposer
    {
        public int Error { get; }
        public string Username { get; }

        public TradingErrorComposer(int Error, string Username)
            : base(Composers.TradingErrorMessageComposer)
        {
            this.Error = Error;
            this.Username = Username;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Error);
            packet.WriteString(Username);
        }
    }
}
