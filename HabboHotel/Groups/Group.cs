using Database_Manager.Database.Session_Details.Interfaces;
using Plus.HabboHotel.Groups;
using StarBlue.Communication.Packets.Outgoing.Messenger;
using StarBlue.HabboHotel.Cache;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Groups
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdminOnlyDeco { get; set; }
        public string Badge { get; set; }
        public int CreateTime { get; set; }
        public int CreatorId { get; set; }
        public string Description { get; set; }
        public int RoomId { get; set; }
        public int Colour1 { get; set; }
        public int Colour2 { get; set; }
        public bool ForumEnabled { get; set; }
        public GroupType GroupType { get; set; }

        private ConcurrentDictionary<int, string> _members;
        private ConcurrentDictionary<int, string> _requests;
        private ConcurrentDictionary<int, string> _administrators;
        internal bool HasForum { get; set; }
        internal bool HasChat { get; set; }

        public Group(int Id, string Name, string Description, string Badge, int RoomId, int Owner, int Time, int Type, int Colour1, int Colour2, int AdminOnlyDeco, bool hforum, bool hChat)
        {
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.RoomId = RoomId;
            this.Badge = Badge;
            CreateTime = Time;
            CreatorId = Owner;
            this.Colour1 = Colour1 == 0 ? 1 : Colour1;
            this.Colour2 = Colour2 == 0 ? 1 : Colour2;
            HasForum = hforum;
            HasChat = hChat;
            GroupType = (GroupType)Type;

            this.AdminOnlyDeco = AdminOnlyDeco;
            ForumEnabled = ForumEnabled;

            _members = new ConcurrentDictionary<int, string>();
            _requests = new ConcurrentDictionary<int, string>();
            _administrators = new ConcurrentDictionary<int, string>();

            InitMembers();
        }

        public void InitMembers()
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetMembers = null;
                dbClient.SetQuery("SELECT `user_id`, `rank`, `joined_time` FROM `group_memberships` WHERE `group_id` = @id");
                dbClient.AddParameter("id", Id);
                GetMembers = dbClient.GetTable();

                if (GetMembers != null)
                {
                    foreach (DataRow Row in GetMembers.Rows)
                    {
                        int UserId = Convert.ToInt32(Row["user_id"]);
                        bool IsAdmin = Convert.ToInt32(Row["rank"]) != 0;

                        if (IsAdmin)
                        {
                            if (!_administrators.ContainsKey(UserId))
                            {
                                _administrators[UserId] = Convert.ToString(Row["joined_time"]);
                            }
                        }
                        else
                        {
                            if (!_members.ContainsKey(UserId))
                            {
                                _members[UserId] = Convert.ToString(Row["joined_time"]);
                            }
                        }
                    }
                }

                DataTable GetRequests = null;
                dbClient.SetQuery("SELECT `user_id`,`joined_time` FROM `group_requests` WHERE `group_id` = @id");
                dbClient.AddParameter("id", Id);
                GetRequests = dbClient.GetTable();

                if (GetRequests != null)
                {
                    foreach (DataRow Row in GetRequests.Rows)
                    {
                        int UserId = Convert.ToInt32(Row["user_id"]);

                        if (_members.ContainsKey(UserId) || _administrators.ContainsKey(UserId))
                        {
                            dbClient.RunFastQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Id + "' AND `user_id` = '" + UserId + "'");
                        }
                        else if (!_requests.ContainsKey(UserId))
                        {
                            _requests[UserId] = Convert.ToString(Row["joined_time"]);
                        }
                    }
                }
            }
        }

        public ConcurrentDictionary<int, string> GetMembers => _members;

        public ConcurrentDictionary<int, string> GetRequests => _requests;

        public ConcurrentDictionary<int, string> GetAdministrators => _administrators;

        public IEnumerable<KeyValuePair<int, string>> GetAllMembers => _administrators.Concat(_members.OrderBy(x => x.Value)).OrderBy(x => x.Value);

        public int MemberCount => _members.Count + _administrators.Count;

        public int RequestCount => _requests.Count;

        public bool IsMember(int Id)
        {
            return _members.ContainsKey(Id) || _administrators.ContainsKey(Id);
        }

        public bool IsAdmin(int Id)
        {
            return _administrators.ContainsKey(Id);
        }

        public bool HasRequest(int Id)
        {
            return _requests.ContainsKey(Id);
        }

        public void MakeAdmin(int Id)
        {
            string Joined = Convert.ToString(DateTime.Now);
            if (_members.ContainsKey(Id))
            {
                _members.TryRemove(Id, out Joined);
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE group_memberships SET `rank` = '1' WHERE `user_id` = @uid AND `group_id` = @gid LIMIT 1");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }

            if (!_administrators.ContainsKey(Id))
            {
                _administrators[Id] = Joined;
            }
        }

        public void TakeAdmin(int UserId)
        {
            if (!_administrators.ContainsKey(UserId))
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE group_memberships SET `rank` = '0' WHERE user_id = @uid AND group_id = @gid");
                dbClient.AddParameter("gid", Id);
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }

            _administrators.TryRemove(UserId, out string Joined);
            _members[UserId] = Joined;
        }

        public void AddMember(int Id)
        {
            if (IsMember(Id) || GroupType == GroupType.LOCKED && _requests.ContainsKey(Id))
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                if (IsAdmin(Id))
                {
                    dbClient.SetQuery("UPDATE `group_memberships` SET `rank` = '0' WHERE user_id = @uid AND group_id = @gid");
                    _administrators.TryRemove(Id, out string Joined);
                    _members[Id] = Convert.ToString(DateTime.Now);
                }
                else if (GroupType == GroupType.LOCKED)
                {
                    dbClient.SetQuery("INSERT INTO `group_requests` (user_id, group_id, joined_time) VALUES (@uid, @gid, @jtime)");
                    _requests[Id] = Convert.ToString(DateTime.Now);
                }
                else
                {
                    dbClient.SetQuery("INSERT INTO `group_memberships` (user_id, group_id, joined_time) VALUES (@uid, @gid, @jtime)");
                    _members[Id] = Convert.ToString(DateTime.Now);
                }

                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.AddParameter("jtime", DateTime.Now);
                dbClient.RunQuery();
            }
        }

        public void DeleteMember(int Id)
        {
            if (IsMember(Id))
            {
                if (_members.ContainsKey(Id))
                {
                    _members.TryRemove(Id, out string Joined);
                }
            }
            else if (IsAdmin(Id))
            {
                if (_administrators.ContainsKey(Id))
                {
                    _administrators.TryRemove(Id, out string Joined);
                }
            }
            else
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM group_memberships WHERE user_id=@uid AND group_id=@gid LIMIT 1");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }
        }

        public void CreateGroupChat(Group group)
        {
            if (group.HasChat)
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE groups SET `has_chat` = '1' WHERE id = @gid");
                dbClient.AddParameter("gid", group.Id);
                dbClient.RunQuery();
            }

            group.HasChat = true;
            List<GameClient> GroupMembers = (from Client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null && IsMember(Client.GetHabbo().Id) select Client).ToList();
            foreach (GameClient Client in GroupMembers)
            {
                if (Client == null)
                {
                    continue;
                }

                Client.SendMessage(new FriendListUpdateComposer(group, 1));
            }

        }

        public GroupMember GetGroupMember(int Id, int GroupId)
        {
            UserCache GroupMember = StarBlueServer.GetGame().GetCacheManager().GenerateUser(Id);
            if (GroupMember != null)
            {
                string Username = GroupMember.Username;
                string Look = GroupMember.Look;
                string JoinedTime = string.Empty;

                GameClient Target = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(GroupMember.Username);
                if (Target != null)
                {
                    if (IsAdmin(Id))
                    {
                        JoinedTime = _administrators[Id];
                    }
                    else if (HasRequest(Id))
                    {
                        JoinedTime = _requests[Id];
                    }
                    else
                    {
                        JoinedTime = _members[Id];
                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("select joined_time from group_memberships where group_id = " + GroupId + " and user_id = " + Id + "");
                        DataRow row = dbClient.GetRow();
                        if (row != null)
                        {
                            JoinedTime = Convert.ToString(row["joined_time"]);
                        }
                    }
                }

                return new GroupMember(Id, Username, Look, JoinedTime);
            }

            return null;
        }

        public void HandleRequest(int Id, bool Accepted)
        {
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                if (Accepted)
                {
                    dbClient.SetQuery("INSERT INTO group_memberships (user_id, group_id, joined_time) VALUES (@uid, @gid, @jtime)");
                    dbClient.AddParameter("gid", this.Id);
                    dbClient.AddParameter("uid", Id);
                    dbClient.AddParameter("jtime", DateTime.Now);
                    dbClient.RunQuery();

                    _members[Id] = Convert.ToString(DateTime.Now);
                }

                dbClient.SetQuery("DELETE FROM group_requests WHERE user_id=@uid AND group_id=@gid LIMIT 1");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }

            if (_requests.ContainsKey(Id))
            {
                _requests.TryRemove(Id, out string value);
            }
        }

        public void ClearRequests()
        {
            _requests.Clear();
        }

        public void Dispose()
        {
            _requests.Clear();
            _members.Clear();
            _administrators.Clear();
        }
    }
}
