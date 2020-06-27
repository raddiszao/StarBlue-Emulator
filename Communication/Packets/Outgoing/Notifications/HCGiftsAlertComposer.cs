namespace StarBlue.Communication.Packets.Outgoing.Rooms.Notifications
{
    class HCGiftsAlertComposer : ServerPacket
    {
        public HCGiftsAlertComposer() : base(ServerPacketHeader.HCGiftsAlertComposer)
        {
            base.WriteInteger(1);
        }
    }
}