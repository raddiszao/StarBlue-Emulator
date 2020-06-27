namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    class CallForHelperErrorComposer : ServerPacket
    {
        public CallForHelperErrorComposer(int errorCode)
            : base(ServerPacketHeader.CallForHelperErrorMessageComposer)
        {
            base.WriteInteger(errorCode);
        }
    }
}
