namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraSendImageUrlMessageComposer : ServerPacket
    {
        public CameraSendImageUrlMessageComposer(string ImageUrl)
            : base(ServerPacketHeader.CameraSendImageUrlMessageComposer)
        {
            base.WriteString(ImageUrl);
        }
    }
}