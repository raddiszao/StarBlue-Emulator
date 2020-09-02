namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class RoomMuteSettingsComposer : ServerPacket
    {
        public RoomMuteSettingsComposer(bool Status)
            : base(ServerPacketHeader.RoomMuteSettingsMessageComposer)
        {
            base.WriteBoolean(Status);
        }
    }
}