namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class HideUserOnPlaying : ServerPacket
    {
        public HideUserOnPlaying(bool state)
            : base(ServerPacketHeader.HideUserOnPlayingComposer)
        {
            base.WriteBoolean(state);
        }
    }
}
