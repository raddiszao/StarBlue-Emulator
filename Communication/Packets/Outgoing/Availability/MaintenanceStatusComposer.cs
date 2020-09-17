namespace StarBlue.Communication.Packets.Outgoing.Availability
{
    internal class MaintenanceStatusComposer : MessageComposer
    {
        private int Minutes { get; }
        private int Duration { get; }

        public MaintenanceStatusComposer(int Minutes, int Duration)
            : base(Composers.MaintenanceStatusMessageComposer)
        {
            this.Minutes = Minutes;
            this.Duration = Duration;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteBoolean(false);
            packet.WriteInteger(Minutes);//Time till shutdown.
            packet.WriteInteger(Duration);//Duration
        }
    }
}