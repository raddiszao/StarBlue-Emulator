namespace StarBlue.Communication.Packets.Outgoing.Rooms.Chat
{
    public class ChatComposer : MessageComposer
    {
        private int VirtualId { get; }
        private string Message { get; }
        private int Emotion { get; }
        private int Colour { get; }

        public ChatComposer(int VirtualId, string Message, int Emotion, int Colour)
            : base(Composers.ChatMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.Message = Message;
            this.Emotion = Emotion;
            this.Colour = Colour;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteString(Message);
            packet.WriteInteger(Emotion);
            packet.WriteInteger(Colour);
            packet.WriteInteger(0);
            packet.WriteInteger(-1);
        }
    }
}