namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni
{
    internal class MysticBoxRewardComposer : MessageComposer
    {
        private string type { get; }
        private int itemID { get; }

        public MysticBoxRewardComposer(string type, int itemID)
            : base(Composers.MysticBoxRewardComposer)
        {
            this.type = type;
            this.itemID = itemID;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(type);
            packet.WriteInteger(itemID);
        }
    }
}