namespace StarBlue.Communication.Packets.Outgoing.Nux
{
    class NuxAlertComposer : ServerPacket
    {
        public NuxAlertComposer(string Message) : base(ServerPacketHeader.NuxAlertMessageComposer)
        {
            base.WriteString(Message);
        }
    }
}
