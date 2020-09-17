namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class SetGroupIdComposer : MessageComposer
    {
        private int GroupId { get; }

        public SetGroupIdComposer(int Id)
            : base(Composers.SetGroupIdMessageComposer)
        {
            this.GroupId = Id;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GroupId);
        }
    }
}
