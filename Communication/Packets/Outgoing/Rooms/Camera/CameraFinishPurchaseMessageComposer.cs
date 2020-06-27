namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraFinishPurchaseMessageComposer : ServerPacket
    {
        public CameraFinishPurchaseMessageComposer()
            : base(ServerPacketHeader.CameraFinishPurchaseMessageComposer)
        {

        }
    }
}