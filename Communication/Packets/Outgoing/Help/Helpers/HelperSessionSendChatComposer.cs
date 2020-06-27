namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    class HelperSessionSendChatComposer : ServerPacket
    {
        public HelperSessionSendChatComposer(int senderId, string message)
            : base(ServerPacketHeader.HelperSessionSendChatMessageComposer)
        {
            base.WriteString(message);
            base.WriteInteger(senderId);
        }
    }
}
