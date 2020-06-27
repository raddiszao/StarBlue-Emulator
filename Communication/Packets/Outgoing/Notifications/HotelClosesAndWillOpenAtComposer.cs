namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    class HotelClosesAndWillOpenAtComposer : ServerPacket
    {
        public HotelClosesAndWillOpenAtComposer(int Hour, int Minute, bool Closed)
            : base(ServerPacketHeader.HotelClosesAndWillOpenAtComposer)
        {
            base.WriteInteger(Hour);
            base.WriteInteger(Minute);
            base.WriteBoolean(true);
        }
    }
}