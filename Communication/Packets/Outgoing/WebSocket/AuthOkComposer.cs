using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class AuthOkComposer : WebComposer
    {
        public AuthOkComposer() : base(1)
        {
            base.WriteString("success");
        }
    }
}