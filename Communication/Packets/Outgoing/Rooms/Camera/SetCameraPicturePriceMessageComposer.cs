namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class SetCameraPicturePriceMessageComposer : ServerPacket
    {
        public SetCameraPicturePriceMessageComposer(int Credits, int Duckets, int PublishPicCost)
            : base(ServerPacketHeader.SetCameraPicturePriceMessageComposer)
        {
            base.WriteInteger(Credits);
            base.WriteInteger(Duckets);
            base.WriteInteger(PublishPicCost);
        }
    }
}