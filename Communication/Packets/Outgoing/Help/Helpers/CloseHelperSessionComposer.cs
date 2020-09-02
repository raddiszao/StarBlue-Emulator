namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    internal class CloseHelperSessionComposer : ServerPacket
    {
        public CloseHelperSessionComposer()
            : base(ServerPacketHeader.CloseHelperSessionMessageComposer)
        { }
    }
}
