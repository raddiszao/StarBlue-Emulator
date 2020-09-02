namespace StarBlue.Communication.Packets.Outgoing.Availability
{
    internal class MaintenanceStatusComposer : ServerPacket
    {
        public MaintenanceStatusComposer(int Minutes, int Duration)
            : base(ServerPacketHeader.MaintenanceStatusMessageComposer)
        {
            base.WriteBoolean(false);
            base.WriteInteger(Minutes);//Time till shutdown.
            base.WriteInteger(Duration);//Duration
        }
    }
}