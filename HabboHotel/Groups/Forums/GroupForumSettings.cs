using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Data;

namespace StarBlue.HabboHotel.Groups.Forums
{
    public class GroupForumSettings
    {
        public GroupForum ParentForum { get; private set; }

        public int WhoCanRead;
        public int WhoCanPost;
        public int WhoCanInitDiscussions;
        public int WhoCanModerate;

        public GroupForumSettings(GroupForum Forum)
        {
            ParentForum = Forum;

            DataRow Row;
            using (IQueryAdapter adap = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("SELECT * FROM group_forums_settings WHERE group_id = @id");
                adap.AddParameter("id", Forum.Id);
                Row = adap.GetRow();
            }

            if (Row == null)
            {
                using (IQueryAdapter adap = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    adap.SetQuery("REPLACE INTO group_forums_settings (group_id) VALUES (@id);SELECT * FROM group_forums_settings WHERE group_id = @id");
                    adap.AddParameter("id", Forum.Id);
                    Row = adap.GetRow();
                }
            }

            WhoCanRead = Convert.ToInt32(Row["who_can_read"]);
            WhoCanPost = Convert.ToInt32(Row["who_can_post"]);
            WhoCanInitDiscussions = Convert.ToInt32(Row["who_can_init_discussions"]);
            WhoCanModerate = Convert.ToInt32(Row["who_can_mod"]);
        }

        public void Save()
        {
            using (IQueryAdapter adap = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("UPDATE group_forums_settings SET who_can_read = @a, who_can_post = @b, who_can_init_discussions = @c, who_can_mod = @d WHERE group_id = @id");
                adap.AddParameter("id", ParentForum.Id);
                adap.AddParameter("a", WhoCanRead);
                adap.AddParameter("b", WhoCanPost);
                adap.AddParameter("c", WhoCanInitDiscussions);
                adap.AddParameter("d", WhoCanModerate);
                adap.RunQuery();
            }
        }

        public GroupForumPermissionLevel GetLevel(int n)
        {
            switch (n)
            {
                case 0:
                default:
                    return GroupForumPermissionLevel.ANYONE;

                case 1:
                    return GroupForumPermissionLevel.JUST_MEMBERS;

                case 2:
                    return GroupForumPermissionLevel.JUST_ADMIN;

                case 3:
                    return GroupForumPermissionLevel.JUST_OWNER;
            }
        }

        public string GetReasonForNot(GameClient Session, int PermissionType)
        {
            if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                return "";
            }

            switch (GetLevel(PermissionType))
            {
                default:
                case GroupForumPermissionLevel.ANYONE:
                    return "";

                case GroupForumPermissionLevel.JUST_ADMIN:
                    return ParentForum.Group.IsAdmin(Session.GetHabbo().Id) ? "" : "not_admin";

                case GroupForumPermissionLevel.JUST_MEMBERS:
                    return ParentForum.Group.IsMember(Session.GetHabbo().Id) ? "" : "not_member";

                case GroupForumPermissionLevel.JUST_OWNER:
                    return ParentForum.Group.CreatorId == Session.GetHabbo().Id ? "" : "not_owner";
            }
        }
    }

    public enum GroupForumPermissionLevel
    {
        ANYONE,
        JUST_MEMBERS,
        JUST_ADMIN,
        JUST_OWNER
    }


}
