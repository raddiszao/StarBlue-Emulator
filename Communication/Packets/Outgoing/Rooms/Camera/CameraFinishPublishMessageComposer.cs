namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraFinishPublishMessageComposer : ServerPacket
    {
        public CameraFinishPublishMessageComposer(bool isOk, int CooldownSeconds, string ExtraData)
            : base(ServerPacketHeader.CameraFinishPublishMessageComposer)
        {
            base.WriteBoolean(isOk);
            base.WriteInteger(CooldownSeconds);
            if (!ExtraData.Equals(""))
            {
                base.WriteString(ExtraData);
            }
        }
    }
}