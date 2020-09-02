namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class NewGroupInfoComposer : ServerPacket
    {
        public NewGroupInfoComposer(int RoomId, int GroupId)
            : base(ServerPacketHeader.NewGroupInfoMessageComposer)
        {
            base.WriteInteger(RoomId);
            base.WriteInteger(GroupId);
        }
    }
}
