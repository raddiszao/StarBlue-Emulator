namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class SetCameraPicturePriceMessageComposer : MessageComposer
    {
        private int Credits { get; }
        private int Duckets { get; }
        private int PublishPicCost { get; }

        public SetCameraPicturePriceMessageComposer(int Credits, int Duckets, int PublishPicCost)
            : base(Composers.SetCameraPicturePriceMessageComposer)
        {
            this.Credits = Credits;
            this.Duckets = Duckets;
            this.PublishPicCost = PublishPicCost;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Credits);
            packet.WriteInteger(Duckets);
            packet.WriteInteger(PublishPicCost);
        }
    }
}