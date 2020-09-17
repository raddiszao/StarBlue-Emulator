using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class ChangeEmojiStateComposer : WebComposer
    {
        public ChangeEmojiStateComposer(string state) : base(10)
        {
            base.WriteString(state);
        }
    }
}