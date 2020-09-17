namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class AlertNotificationHCMessageComposer : MessageComposer
    {
        private int type { get; }

        public AlertNotificationHCMessageComposer(int type)
            : base(Composers.AlertNotificationHCMessageComposer)
        {
            this.type = type;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(type);
        }
    }
}
