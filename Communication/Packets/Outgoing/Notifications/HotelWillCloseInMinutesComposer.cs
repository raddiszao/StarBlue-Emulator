namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class HotelWillCloseInMinutesComposer : ServerPacket
    {
        public HotelWillCloseInMinutesComposer(int Minutes)
            : base(ServerPacketHeader.HotelWillCloseInMinutesComposer)
        {
            base.WriteInteger(Minutes);
        }
    }
}