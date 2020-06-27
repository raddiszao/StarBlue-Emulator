namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    class GuardianHandleReportComposer : ServerPacket
    {
        public GuardianHandleReportComposer(int seconds)
            : base(ServerPacketHeader.GuardianHandleReportMessageComposer)
        {
            base.WriteInteger(seconds);
        }
    }
}
