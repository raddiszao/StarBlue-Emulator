namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    class WiredSmartAlertComposer : ServerPacket
    {
        public WiredSmartAlertComposer(string Message)
            : base(ServerPacketHeader.WiredSmartAlertComposer)

        {
            base.WriteString(Message);
        }
    }
}