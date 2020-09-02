using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Groups.Forums;

namespace StarBlue.Communication.Packets.Outgoing.Groups
{
    internal class ThreadReplyComposer : ServerPacket
    {
        public ThreadReplyComposer(GameClient Session, GroupForumThreadPost Post)
            : base(ServerPacketHeader.ThreadReplyMessageComposer)
        {
            HabboHotel.Users.Habbo User = Post.GetAuthor();
            base.WriteInteger(Post.ParentThread.ParentForum.Id);
            base.WriteInteger(Post.ParentThread.Id);

            base.WriteInteger(Post.Id); //Post Id
            base.WriteInteger(Post.ParentThread.Posts.IndexOf(Post)); //Post Index

            base.WriteInteger(User.Id); //User id
            base.WriteString(User.Username); //Username
            base.WriteString(User.Look); //User look

            base.WriteInteger((int)(StarBlueServer.GetUnixTimestamp() - Post.Timestamp)); //User message timestamp
            base.WriteString(Post.Message); // Message text
            base.WriteByte(0); // User message oculted by - level
            base.WriteInteger(0); // User that oculted message ID
            base.WriteString(""); //Oculted message user name
            base.WriteInteger(10);
            base.WriteInteger(Post.ParentThread.GetUserPosts(User.Id).Count); //User messages count
        }
    }
}
