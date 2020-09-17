namespace StarBlue.Communication.Packets.Outgoing.Notifications
{
    internal class HotelClosedAndOpensComposer : MessageComposer
    {
        private int Hour { get; }
        private int Minute { get; }

        public HotelClosedAndOpensComposer(int Hour, int Minute)
            : base(Composers.HotelClosedAndOpensComposer)
        {
            this.Hour = Hour;
            this.Minute = Minute;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Hour);
            packet.WriteInteger(Minute);
        }
    }
}