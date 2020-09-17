namespace StarBlue.Communication.Packets.Outgoing.Rooms.Camera
{
    public class CameraFinishPublishMessageComposer : MessageComposer
    {
        private bool isOk { get; }
        private int CooldownSeconds { get; }
        private string ExtraData { get; }

        public CameraFinishPublishMessageComposer(bool isOk, int CooldownSeconds, string ExtraData)
            : base(Composers.CameraFinishPublishMessageComposer)
        {
            this.isOk = isOk;
            this.CooldownSeconds = CooldownSeconds;
            this.ExtraData = ExtraData;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(isOk);
            packet.WriteInteger(CooldownSeconds);
            if (!ExtraData.Equals(""))
            {
                packet.WriteString(ExtraData);
            }
        }
    }
}