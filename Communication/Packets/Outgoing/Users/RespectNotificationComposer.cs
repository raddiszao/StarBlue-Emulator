namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class RespectNotificationComposer : ServerPacket
    {
        public RespectNotificationComposer(int userID, int Respect)
            : base(ServerPacketHeader.RespectNotificationMessageComposer)
        {
            base.WriteInteger(userID);
            base.WriteInteger(Respect);
        }
    }
}
