namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    class HotelWillCloseInMinutesComposer : ServerPacket
    {
        public HotelWillCloseInMinutesComposer(int Minutes)
            : base(ServerPacketHeader.HotelWillCloseInMinutesComposer)
        {
            base.WriteInteger(Minutes);
        }
    }
}