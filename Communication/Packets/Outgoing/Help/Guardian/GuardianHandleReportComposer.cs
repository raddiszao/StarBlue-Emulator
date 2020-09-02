namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class GuardianHandleReportComposer : ServerPacket
    {
        public GuardianHandleReportComposer(int seconds)
            : base(ServerPacketHeader.GuardianHandleReportMessageComposer)
        {
            base.WriteInteger(seconds);
        }
    }
}
