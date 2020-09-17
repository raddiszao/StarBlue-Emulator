namespace StarBlue.Communication.Packets.Outgoing.Users
{
    internal class UserTagsComposer : MessageComposer
    {
        private int UserId { get; }

        public UserTagsComposer(int UserId)
            : base(Composers.UserTagsMessageComposer)
        {
            this.UserId = UserId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(UserId);
            packet.WriteInteger(0);
        }
    }
}
