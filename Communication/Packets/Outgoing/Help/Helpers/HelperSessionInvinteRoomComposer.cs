namespace StarBlue.Communication.Packets.Outgoing.Help.Helpers
{
    class HelperSessionInvinteRoomComposer : ServerPacket
    {
        public HelperSessionInvinteRoomComposer(int int1, string str)
            : base(ServerPacketHeader.HelperSessionInvinteRoomMessageComposer)
        {
            base.WriteInteger(int1);
            base.WriteString(str);
        }
    }
}
