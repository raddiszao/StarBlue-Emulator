namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class PinVerifyComposer : ServerPacket
    {
        public PinVerifyComposer(string type) : base(12)
        {
            base.WriteString(type);
        }
    }
}