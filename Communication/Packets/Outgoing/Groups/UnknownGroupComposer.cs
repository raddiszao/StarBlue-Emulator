namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class UnknownGroupComposer : MessageComposer
    {
        public int GroupId { get; }
        public int HabboId { get; }

        public UnknownGroupComposer(int GroupId, int HabboId)
            : base(Composers.UnknownGroupMessageComposer)
        {
            this.GroupId = GroupId;
            this.HabboId = HabboId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GroupId);
            packet.WriteInteger(HabboId);
        }
    }
}