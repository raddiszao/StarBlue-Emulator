namespace StarBlue.Communication.Packets.Outgoing.Handshake
{
    internal class GenericErrorComposer : ServerPacket
    {
        public GenericErrorComposer(int errorId)
            : base(ServerPacketHeader.GenericErrorMessageComposer)
        {
            base.WriteInteger(errorId);
        }
    }
}
