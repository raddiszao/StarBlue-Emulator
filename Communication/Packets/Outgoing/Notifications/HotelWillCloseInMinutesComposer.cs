namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class HotelWillCloseInMinutesComposer : MessageComposer
    {
        private int Minutes { get; }

        public HotelWillCloseInMinutesComposer(int Minutes)
            : base(Composers.HotelWillCloseInMinutesComposer)
        {
            this.Minutes = Minutes;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Minutes);
        }
    }
}