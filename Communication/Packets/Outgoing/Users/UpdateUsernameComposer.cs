namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class UpdateUsernameComposer : ServerPacket
    {
        public UpdateUsernameComposer(string User)
            : base(ServerPacketHeader.UpdateUsernameMessageComposer)
        {
            base.WriteInteger(0);
            base.WriteString(User);
            base.WriteInteger(0);
        }
    }
}