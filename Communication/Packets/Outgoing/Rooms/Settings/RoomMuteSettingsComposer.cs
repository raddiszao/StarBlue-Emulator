namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomMuteSettingsComposer : MessageComposer
    {
        public bool Status { get; }
        public RoomMuteSettingsComposer(bool Status)
            : base(Composers.RoomMuteSettingsMessageComposer)
        {
            this.Status = Status;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(Status);
        }
    }
}