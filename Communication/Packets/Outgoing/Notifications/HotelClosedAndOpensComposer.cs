namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class HotelClosedAndOpensComposer : ServerPacket
    {
        public HotelClosedAndOpensComposer(int Hour, int Minute)
            : base(ServerPacketHeader.HotelClosedAndOpensComposer)
        {
            base.WriteInteger(Hour);
            base.WriteInteger(Minute);
        }
    }
}