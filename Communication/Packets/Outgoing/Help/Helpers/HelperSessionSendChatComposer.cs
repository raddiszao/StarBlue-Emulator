namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class HelperSessionSendChatComposer : MessageComposer
    {
        private int senderId { get; }
        private string message { get; }

        public HelperSessionSendChatComposer(int senderId, string message)
            : base(Composers.HelperSessionSendChatMessageComposer)
        {
            this.senderId = senderId;
            this.message = message;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(message);
            packet.WriteInteger(senderId);
        }
    }
}
