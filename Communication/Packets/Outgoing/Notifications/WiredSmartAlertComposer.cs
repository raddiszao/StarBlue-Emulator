namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class WiredSmartAlertComposer : ServerPacket
    {
        public WiredSmartAlertComposer(string Message)
            : base(ServerPacketHeader.WiredSmartAlertComposer)

        {
            base.WriteString(Message);
        }
    }
}