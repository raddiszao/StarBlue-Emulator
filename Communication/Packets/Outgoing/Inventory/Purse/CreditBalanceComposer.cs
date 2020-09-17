namespace StarBlue.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class CreditBalanceComposer : MessageComposer
    {
        public int CreditsBalance { get; }

        public CreditBalanceComposer(int creditsBalance)
            : base(Composers.CreditBalanceMessageComposer)
        {
            this.CreditsBalance = creditsBalance;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(CreditsBalance + ".0");
        }
    }
}
