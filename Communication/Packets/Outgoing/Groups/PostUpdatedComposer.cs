using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class PostUpdatedComposer : MessageComposer
    {
        private GroupForumThreadPost Post { get; }

        public PostUpdatedComposer(GroupForumThreadPost Post)
            : base(Composers.PostUpdatedMessageComposer)
        {
            this.Post = Post;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Post.ParentThread.ParentForum.Id);
            packet.WriteInteger(Post.ParentThread.Id);

            Post.SerializeData(packet);
        }
    }
}
