namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class UnknownGroupComposer : ServerPacket
    {
        public UnknownGroupComposer(int GroupId, int HabboId)
            : base(ServerPacketHeader.UnknownGroupMessageComposer)
        {
            base.WriteInteger(GroupId);
            base.WriteInteger(HabboId);
        }
    }
}