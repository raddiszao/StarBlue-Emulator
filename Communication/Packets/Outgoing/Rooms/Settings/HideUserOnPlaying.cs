namespace StarBlue.Communication.Packets.Outgoing.Rooms.Settings
{
    internal class HideUserOnPlaying : MessageComposer
    {
        private bool state { get; }

        public HideUserOnPlaying(bool state)
            : base(Composers.HideUserOnPlayingComposer)
        {
            this.state = state;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(state);
        }
    }
}
