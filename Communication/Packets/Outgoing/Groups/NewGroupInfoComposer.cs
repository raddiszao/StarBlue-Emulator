namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class NewGroupInfoComposer : MessageComposer
    {
        public int RoomId { get; }
        public int GroupId { get; }

        public NewGroupInfoComposer(int RoomId, int GroupId)
            : base(Composers.NewGroupInfoMessageComposer)
        {
            this.RoomId = RoomId;
            this.GroupId = GroupId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(RoomId);
            packet.WriteInteger(GroupId);
        }
    }
}
