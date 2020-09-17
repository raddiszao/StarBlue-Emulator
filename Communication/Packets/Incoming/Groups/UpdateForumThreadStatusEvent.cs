using StarBlue.HabboHotel.GameClients;

namespace StarBlue.Communication.Packets.Incoming.Groups
{
    public class UpdateForumThreadStatusEvent : IPacketEvent
    {
        public void Parse(GameClient Session, MessageEvent Packet)
        {
            int ForumID = Packet.PopInt();
            int ThreadID = Packet.PopInt();
            bool Pinned = Packet.PopBoolean();
            bool Locked = Packet.PopBoolean();


            HabboHotel.Groups.Forums.GroupForum forum = StarBlueServer.GetGame().GetGroupForumManager().GetForum(ForumID);
            HabboHotel.Groups.Forums.GroupForumThread thread = forum.GetThread(ThreadID);

            if (forum.Settings.GetReasonForNot(Session, forum.Settings.WhoCanModerate) != "")
            {
                Session.SendNotification(("forums.thread.update.error.rights"));
                return;
            }

            bool isPining = thread.Pinned != Pinned,
                isLocking = thread.Locked != Locked;

            thread.Pinned = Pinned;
            thread.Locked = Locked;

            thread.Save();

            Session.SendMessage(new Outgoing.Groups.ThreadUpdatedComposer(Session, thread));

            if (isPining)
            {
                if (Pinned)
                {
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.pinned"));
                }
                else
                {
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.unpinned"));
                }
            }

            if (isLocking)
            {
                if (Locked)
                {
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.locked"));
                }
                else
                {
                    Session.SendMessage(new Outgoing.Rooms.Notifications.RoomNotificationComposer("forums.thread.unlocked"));
                }
            }
        }
    }
}
