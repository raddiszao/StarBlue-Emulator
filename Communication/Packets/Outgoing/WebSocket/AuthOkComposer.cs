namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class AuthOkComposer : ServerPacket
    {
        public AuthOkComposer() : base(1)
        {
            base.WriteString("success");
        }
    }
}