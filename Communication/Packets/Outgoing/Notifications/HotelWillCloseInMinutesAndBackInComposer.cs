namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class HotelWillCloseInMinutesAndBackInComposer : MessageComposer
    {
        private int CloseIn { get; }
        private int OpenIn { get; }

        public HotelWillCloseInMinutesAndBackInComposer(int CloseIn, int OpenIn)
            : base(Composers.HotelWillCloseInMinutesAndBackInComposer)
        {
            this.CloseIn = CloseIn;
            this.OpenIn = OpenIn;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(true);
            packet.WriteInteger(CloseIn);
            packet.WriteInteger(OpenIn);
        }
    }
}