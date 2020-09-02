namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class HelperSessionChatIsTypingComposer : ServerPacket
    {
        public HelperSessionChatIsTypingComposer(bool typing)
            : base(ServerPacketHeader.HelperSessionChatIsTypingMessageComposer)
        {
            base.WriteBoolean(typing);
        }
    }
}
