using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class PinVerifyComposer : WebComposer
    {
        public PinVerifyComposer(string type) : base(12)
        {
            base.WriteString(type);
        }
    }
}