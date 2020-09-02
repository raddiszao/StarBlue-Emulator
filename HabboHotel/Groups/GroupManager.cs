using log4net;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Users;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Groups
{
    public class GroupManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Groups.GroupManager");

        public Dictionary<int, GroupBackGroundColours> BackGroundColours;
        public List<GroupBaseColours> BaseColours;
        public List<GroupBases> Bases;

        public Dictionary<int, GroupSymbolColours> SymbolColours;
        public List<GroupSymbols> Symbols;

        private readonly object _groupLoadingSync;
        private ConcurrentDictionary<int, Group> _groups;

        public GroupManager()
        {
            _groupLoadingSync = new object();

            Init();
        }

        public bool TryGetGroup(int Id, out Group Group)
        {
            Group = null;

            if (_groups.ContainsKey(Id))
            {
                return _groups.TryGetValue(Id, out Group);
            }

            lock (_groupLoadingSync)
            {
                if (_groups.ContainsKey(Id))
                {
                    return _groups.TryGetValue(Id, out Group);
                }

                DataRow Row = null;
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", Id);
                    Row = dbClient.GetRow();

                    if (Row != null)
                    {
                        int created;
                        try
                        {
                            int.TryParse(Row["created"].ToString(), out created);
                        }
                        catch { created = StarBlueServer.GetIUnixTimestamp(); }
                        Group = new Group(
                            Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["desc"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["owner_id"]),
                            created, Convert.ToInt32(Row["state"]), Convert.ToInt32(Row["colour1"]), Convert.ToInt32(Row["colour2"]), Convert.ToInt32(Row["admindeco"]), Convert.ToInt32(Row["has_forum"]) == 1, Convert.ToInt32(Row["has_chat"]) == 1);
                        _groups.TryAdd(Group.Id, Group);
                        return true;
                    }
                }
            }
            return false;
        }

        public void Init()
        {
            Bases = new List<GroupBases>();
            Symbols = new List<GroupSymbols>();
            BaseColours = new List<GroupBaseColours>();
            SymbolColours = new Dictionary<int, GroupSymbolColours>();
            BackGroundColours = new Dictionary<int, GroupBackGroundColours>();
            _groups = new ConcurrentDictionary<int, Group>();

            ClearInfo();
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM groups_items WHERE enabled='1'");
                DataTable dItems = dbClient.GetTable();

                foreach (DataRow dRow in dItems.Rows)
                {
                    switch (dRow[0].ToString())
                    {
                        case "base":
                            Bases.Add(new GroupBases(Convert.ToInt32(dRow[1]), dRow[2].ToString(), dRow[3].ToString()));
                            break;

                        case "symbol":
                            Symbols.Add(new GroupSymbols(Convert.ToInt32(dRow[1]), dRow[2].ToString(), dRow[3].ToString()));
                            break;

                        case "color":
                            BaseColours.Add(new GroupBaseColours(Convert.ToInt32(dRow[1]), dRow[2].ToString()));
                            break;

                        case "color2":
                            SymbolColours.Add(Convert.ToInt32(dRow[1]), new GroupSymbolColours(Convert.ToInt32(dRow[1]), dRow[2].ToString()));
                            break;

                        case "color3":
                            BackGroundColours.Add(Convert.ToInt32(dRow[1]), new GroupBackGroundColours(Convert.ToInt32(dRow[1]), dRow[2].ToString()));
                            break;
                    }
                }
            }

            log.Info(">> Group Manager -> READY!");
        }

        public void ClearInfo()
        {
            Bases.Clear();
            Symbols.Clear();
            BaseColours.Clear();
            SymbolColours.Clear();
            BackGroundColours.Clear();
        }

        public bool TryCreateGroup(Habbo Player, string Name, string Description, int RoomId, string Badge, int Colour1, int Colour2, out Group Group)
        {
            Group = new Group(0, Name, Description, Badge, RoomId, Player.Id, (int)StarBlueServer.GetUnixTimestamp(), 0, Colour1, Colour2, 0, false, false);
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Badge))
            {
                return false;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `groups` (`name`, `desc`, `badge`, `owner_id`, `created`, `room_id`, `state`, `colour1`, `colour2`, `admindeco`) VALUES (@name, @desc, @badge, @owner, UNIX_TIMESTAMP(), @room, '0', @colour1, @colour2, '0')");
                dbClient.AddParameter("name", Group.Name);
                dbClient.AddParameter("desc", Group.Description);
                dbClient.AddParameter("owner", Group.CreatorId);
                dbClient.AddParameter("badge", Group.Badge);
                dbClient.AddParameter("room", Group.RoomId);
                dbClient.AddParameter("colour1", Group.Colour1);
                dbClient.AddParameter("colour2", Group.Colour2);
                Group.Id = Convert.ToInt32(dbClient.InsertQuery());

                Group.AddMember(Player.Id);
                Group.MakeAdmin(Player.Id);

                if (!_groups.TryAdd(Group.Id, Group))
                {
                    return false;
                }
                else
                {
                    dbClient.SetQuery("UPDATE `rooms` SET `group_id` = @gid WHERE `id` = @rid LIMIT 1");
                    dbClient.AddParameter("gid", Group.Id);
                    dbClient.AddParameter("rid", Group.RoomId);
                    dbClient.RunQuery();

                    dbClient.RunFastQuery("DELETE FROM `room_rights` WHERE `room_id` = '" + RoomId + "'");
                }
            }
            return true;
        }


        public string CheckActiveSymbol(string Symbol)
        {
            if (Symbol == "s000" || Symbol == "s00000")
            {
                return "";
            }
            return Symbol;
        }

        public string GetGroupColour(int Index, bool Colour1)
        {
            if (Colour1)
            {
                if (SymbolColours.ContainsKey(Index))
                {
                    return SymbolColours[Index].Colour;
                }
            }
            else
            {
                if (BackGroundColours.ContainsKey(Index))
                {
                    return BackGroundColours[Index].Colour;
                }
            }

            return "4f8a00";
        }

        public void DeleteGroup(int Id)
        {
            Group Group = null;
            if (_groups.ContainsKey(Id))
            {
                _groups.TryRemove(Id, out Group);
            }

            if (Group != null)
            {
                Group.Dispose();
            }
        }

        public List<Group> GetGroupsForUser(int UserId)
        {
            List<Group> Groups = new List<Group>();
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT g.id FROM group_memberships AS m RIGHT JOIN groups AS g ON m.group_id = g.id WHERE m.user_id = @user ORDER BY g.id DESC");
                dbClient.AddParameter("user", UserId);
                DataTable GetGroups = dbClient.GetTable();

                if (GetGroups != null)
                {
                    foreach (DataRow Row in GetGroups.Rows)
                    {
                        if (TryGetGroup(Convert.ToInt32(Row["id"]), out Group Group))
                        {
                            Groups.Add(Group);
                        }
                    }
                }
            }
            return Groups;
        }
    }
}