namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class NewYearResolutionCompletedComposer : MessageComposer
    {
        private string badge { get; }

        public NewYearResolutionCompletedComposer(string badge)
            : base(Composers.NewYearResolutionCompletedComposer)
        {
            this.badge = badge;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(badge);
            packet.WriteString(badge);
        }
    }
}

