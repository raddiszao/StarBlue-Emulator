using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class DeleteGroupPostEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            int forumId = Packet.PopInt();
            int threadId = Packet.PopInt();
            int postId = Packet.PopInt();
            int deleteLevel = Packet.PopInt();

            HabboHotel.Groups.Forums.GroupForum forum = StarBlueServer.GetGame().GetGroupForumManager().GetForum(forumId);

            HabboHotel.Groups.Forums.GroupForumThread thread = forum.GetThread(threadId);

            HabboHotel.Groups.Forums.GroupForumThreadPost post = thread.GetPost(postId);

            post.DeletedLevel = deleteLevel / 10;
            post.DeleterId = Session.GetHabbo().Id;
            post.Save();
            Session.SendMessage(new PostUpdatedComposer(post));

            if (post.DeletedLevel != 0)
            {
                Session.SendMessage(new RoomNotificationComposer("forums.message.hidden"));
            }
            else
            {
                Session.SendMessage(new RoomNotificationComposer("forums.message.restored"));
            }
        }
    }
}
