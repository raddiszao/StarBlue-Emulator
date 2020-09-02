namespace StarBlue.Communication.Packets.Outgoing.Messenger
{
    internal class MessengerErrorComposer : ServerPacket
    {
        public MessengerErrorComposer(int ErrorCode1, int ErrorCode2)
            : base(ServerPacketHeader.MessengerErrorMessageComposer)
        {
            base.WriteInteger(ErrorCode1);
            base.WriteInteger(ErrorCode2);
        }
    }
}
