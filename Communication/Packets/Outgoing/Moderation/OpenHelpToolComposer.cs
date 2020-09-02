namespace StarBlue.Communication.Packets.Outgoing.Moderation
{
    internal class OpenHelpToolComposer : ServerPacket
    {
        public OpenHelpToolComposer()
            : base(ServerPacketHeader.OpenHelpToolMessageComposer)
        {
            base.WriteInteger(0);
        }
    }
}
