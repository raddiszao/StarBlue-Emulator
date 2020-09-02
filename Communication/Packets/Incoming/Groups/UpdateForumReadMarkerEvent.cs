using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class UpdateForumReadMarkerEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int length = Packet.PopInt();
            for (int i = 0; i < length; i++)
            {
                int forumid = Packet.PopInt(); //Forum ID
                int postid = Packet.PopInt(); //Post ID
                bool readall = Packet.PopBoolean(); //Make all read

                HabboHotel.Groups.Forums.GroupForum forum = StarBlueServer.GetGame().GetGroupForumManager().GetForum(forumid);
                if (forum == null)
                {
                    continue;
                }

                HabboHotel.Groups.Forums.GroupForumThreadPost post = forum.GetPost(postid);
                if (post == null)
                {
                    continue;
                }

                HabboHotel.Groups.Forums.GroupForumThread thread = post.ParentThread;
                int index = thread.Posts.IndexOf(post);
                thread.AddView(Session.GetHabbo().Id, index + 1);

            }
            //Thread.AddView(Session.GetHabbo().Id);
        }
    }
}
