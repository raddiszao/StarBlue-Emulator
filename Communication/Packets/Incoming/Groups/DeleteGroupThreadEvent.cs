using StarBlue.Communication.Packets.Outgoing.Groups;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    internal class DeleteGroupThreadEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int int1 = Packet.PopInt();
            int int2 = Packet.PopInt();
            int int3 = Packet.PopInt();

            HabboHotel.Groups.Forums.GroupForum forum = StarBlueServer.GetGame().GetGroupForumManager().GetForum(int1);

            if (forum == null)
            {
                Session.SendNotification(("forums.thread.delete.error.forumnotfound"));
                return;
            }

            if (forum.Settings.GetReasonForNot(Session, forum.Settings.WhoCanModerate) != "")
            {
                Session.SendNotification(("forums.thread.delete.error.rights"));
                return;
            }

            HabboHotel.Groups.Forums.GroupForumThread thread = forum.GetThread(int2);
            if (thread == null)
            {
                Session.SendNotification(("forums.thread.delete.error.threadnotfound"));
                return;
            }

            thread.DeletedLevel = int3 / 10;

            thread.DeleterUserId = thread.DeletedLevel != 0 ? Session.GetHabbo().Id : 0;

            thread.Save();

            Session.SendMessage(new ThreadsListDataComposer(forum, Session));

            if (thread.DeletedLevel != 0)
            {
                Session.SendMessage(new RoomNotificationComposer("forums.thread.hidden"));
            }
            else
            {
                Session.SendMessage(new RoomNotificationComposer("forums.thread.restored"));
            }
        }
    }
}
