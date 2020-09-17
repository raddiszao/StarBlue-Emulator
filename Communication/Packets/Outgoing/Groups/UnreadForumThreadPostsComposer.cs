namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class UnreadForumThreadPostsComposer : MessageComposer
    {
        private int count { get; }

        public UnreadForumThreadPostsComposer(int count)
            : base(Composers.UnreadForumThreadPostsMessageComposer)
        {
            this.count = count;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(count);
        }
    }
}


