namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class HotelClosesAndWillOpenAtComposer : MessageComposer
    {
        private int Hour { get; }
        private int Minute { get; }
        private bool Closed { get; }

        public HotelClosesAndWillOpenAtComposer(int Hour, int Minute, bool Closed)
            : base(Composers.HotelClosesAndWillOpenAtComposer)
        {
            this.Hour = Hour;
            this.Minute = Minute;
            this.Closed = Closed;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Hour);
            packet.WriteInteger(Minute);
            packet.WriteBoolean(Closed);
        }
    }
}