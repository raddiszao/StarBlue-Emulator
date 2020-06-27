namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    class HelperSessionChatIsTypingComposer : ServerPacket
    {
        public HelperSessionChatIsTypingComposer(bool typing)
            : base(ServerPacketHeader.HelperSessionChatIsTypingMessageComposer)
        {
            base.WriteBoolean(typing);
        }
    }
}
