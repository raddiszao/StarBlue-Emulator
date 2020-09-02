namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class AlertNotificationHCMessageComposer : ServerPacket
    {
        public AlertNotificationHCMessageComposer(int type)
            : base(ServerPacketHeader.AlertNotificationHCMessageComposer)
        {
            base.WriteInteger(type);
        }
    }
}
