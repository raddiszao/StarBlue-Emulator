namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class SendRoomThumbnailAlertMessageComposer : ServerPacket
    {
        public SendRoomThumbnailAlertMessageComposer()
            : base(ServerPacketHeader.SendRoomThumbnailAlertMessageComposer)
        {

        }
    }
}