namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class HelperSessionChatIsTypingComposer : MessageComposer
    {
        private bool typing { get; }

        public HelperSessionChatIsTypingComposer(bool typing)
            : base(Composers.HelperSessionChatIsTypingMessageComposer)
        {
            this.typing = typing;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(typing);
        }
    }
}
