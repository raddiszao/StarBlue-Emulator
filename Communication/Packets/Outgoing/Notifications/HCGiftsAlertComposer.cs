namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class HCGiftsAlertComposer : ServerPacket
    {
        public HCGiftsAlertComposer() : base(ServerPacketHeader.HCGiftsAlertComposer)
        {
            base.WriteInteger(1);
        }
    }
}