namespace StarBlue.Communication.Packets.Outgoing.Rooms.Chat
{
    public class WhisperComposer : MessageComposer
    {
        private int VirtualId { get; }
        private string Text { get; }
        private int Emotion { get; }
        private int Colour { get; }

        public WhisperComposer(int VirtualId, string Text, int Emotion, int Colour)
            : base(Composers.WhisperMessageComposer)
        {
            this.VirtualId = VirtualId;
            this.Text = Text;
            this.Emotion = Emotion;
            this.Colour = Colour;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteString(Text);
            packet.WriteInteger(Emotion);
            packet.WriteInteger(Colour);

            packet.WriteInteger(0);
            packet.WriteInteger(-1);
        }
    }
}