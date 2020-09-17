namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class HCGiftsAlertComposer : MessageComposer
    {
        public HCGiftsAlertComposer() : base(Composers.HCGiftsAlertComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
        }
    }
}