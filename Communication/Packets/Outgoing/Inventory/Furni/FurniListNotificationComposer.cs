namespace StarBlue.Communication.Packets.Outgoing.Inventory.Furni
{
    internal class FurniListNotificationComposer : MessageComposer
    {
        public int FurniId { get; }
        public int Type { get; }

        public FurniListNotificationComposer(int Id, int Type)
            : base(Composers.FurniListNotificationMessageComposer)
        {
            this.FurniId = Id;
            this.Type = Type;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(Type);
            packet.WriteInteger(1);
            packet.WriteInteger(FurniId);
        }
    }
}