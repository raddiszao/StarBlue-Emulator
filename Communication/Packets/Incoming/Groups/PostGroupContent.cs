using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class PostGroupContentEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int ForumId = Packet.PopInt();
            int ThreadId = Packet.PopInt();
            string Caption = Packet.PopString();
            string Message = Packet.PopString();

            HabboHotel.Groups.Forums.GroupForum Forum = StarBlueServer.GetGame().GetGroupForumManager().GetForum(ForumId);
            if (Forum == null)
            {
                Session.SendNotification(";forum.thread.post.reply.error.forumnotfound");
                return;
            }
            string e = "";
            bool IsNewThread = ThreadId == 0;
            if (IsNewThread)
            {

                if ((e = Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanInitDiscussions)) != "")
                {
                    Session.SendNotification(";forum.thread.create.error." + e);
                    return;
                }

                HabboHotel.Groups.Forums.GroupForumThread Thread = Forum.CreateThread(Session.GetHabbo().Id, Caption);
                HabboHotel.Groups.Forums.GroupForumThreadPost Post = Thread.CreatePost(Session.GetHabbo().Id, Message);

                Session.SendMessage(new ThreadCreatedComposer(Session, Thread));

                Session.SendMessage(new ThreadDataComposer(Thread, 0, 2000));

                //Session.SendMessage(new PostUpdatedComposer(Session, Post));
                //Session.SendMessage(new ThreadReplyComposer(Session, Post));

                Thread.AddView(Session.GetHabbo().Id, 1);

            }
            else
            {
                HabboHotel.Groups.Forums.GroupForumThread Thread = Forum.GetThread(ThreadId);
                if (Thread == null)
                {
                    Session.SendNotification(";forum.thread.post.reply.error.threadnotfound");
                    return;
                }

                if (Thread.Locked && Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanModerate) != "")
                {
                    Session.SendNotification(";forum.thread.post.reply.error.locked");
                    return;
                }

                if ((e = Forum.Settings.GetReasonForNot(Session, Forum.Settings.WhoCanPost)) != "")
                {
                    Session.SendNotification(";forum.thread.post.reply.error." + e);
                    return;
                }

                HabboHotel.Groups.Forums.GroupForumThreadPost Post = Thread.CreatePost(Session.GetHabbo().Id, Message);
                Session.SendMessage(new ThreadReplyComposer(Session, Post));
                StarBlueServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_Forum", 1);
            }


        }
    }
}
