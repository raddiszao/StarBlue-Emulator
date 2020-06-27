namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    class MOTDNotificationComposer : ServerPacket
    {
        public MOTDNotificationComposer(string Message)
            : base(ServerPacketHeader.MOTDNotificationMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteString(Message);
        }
    }
}
