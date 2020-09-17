using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ThreadReplyComposer : MessageComposer
    {
        private GroupForumThreadPost Post { get; }

        public ThreadReplyComposer(GroupForumThreadPost Post)
            : base(Composers.ThreadReplyMessageComposer)
        {
            this.Post = Post;
        }

        public override void Compose(Composer packet)
        {
            HabboHotel.Users.Habbo User = Post.GetAuthor();
            packet.WriteInteger(Post.ParentThread.ParentForum.Id);
            packet.WriteInteger(Post.ParentThread.Id);

            packet.WriteInteger(Post.Id); //Post Id
            packet.WriteInteger(Post.ParentThread.Posts.IndexOf(Post)); //Post Index

            packet.WriteInteger(User.Id); //User id
            packet.WriteString(User.Username); //Username
            packet.WriteString(User.Look); //User look

            packet.WriteInteger((int)(StarBlueServer.GetUnixTimestamp() - Post.Timestamp)); //User message timestamp
            packet.WriteString(Post.Message); // Message text
            packet.WriteByte(0); // User message oculted by - level
            packet.WriteInteger(0); // User that oculted message ID
            packet.WriteString(""); //Oculted message user name
            packet.WriteInteger(10);
            packet.WriteInteger(Post.ParentThread.GetUserPosts(User.Id).Count); //User messages count
        }
    }
}
