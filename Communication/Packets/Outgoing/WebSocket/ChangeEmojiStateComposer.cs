namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class ChangeEmojiStateComposer : ServerPacket
    {
        public ChangeEmojiStateComposer(string state) : base(10)
        {
            base.WriteString(state);
        }
    }
}