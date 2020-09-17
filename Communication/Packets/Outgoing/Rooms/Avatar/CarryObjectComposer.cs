namespace StarBlue.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class CarryObjectComposer : MessageComposer
    {
        private int virtualID { get; }
        private int itemID { get; }

        public CarryObjectComposer(int virtualID, int itemID)
            : base(Composers.CarryObjectMessageComposer)
        {
            this.virtualID = virtualID;
            this.itemID = itemID;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(virtualID);
            packet.WriteInteger(itemID);
        }
    }
}
