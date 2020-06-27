namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    class HotelWillCloseInMinutesAndBackInComposer : ServerPacket
    {
        public HotelWillCloseInMinutesAndBackInComposer(int CloseIn, int OpenIn)
            : base(ServerPacketHeader.HotelWillCloseInMinutesAndBackInComposer)
        {
            base.WriteBoolean(true);
            base.WriteInteger(CloseIn);
            base.WriteInteger(OpenIn);
        }
    }
}