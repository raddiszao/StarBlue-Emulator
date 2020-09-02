namespace StarBlue.Communication.Packets.Outgoing.Inventory.Purse
{
    internal class CreditBalanceComposer : ServerPacket
    {
        public CreditBalanceComposer(int creditsBalance)
            : base(ServerPacketHeader.CreditBalanceMessageComposer)
        {
            base.WriteString(creditsBalance + ".0");
        }
    }
}
