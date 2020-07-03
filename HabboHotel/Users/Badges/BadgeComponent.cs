using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Inventory.Badges;
using StarBlue.Communication.Packets.Outgoing.Inventory.Furni;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users.UserDataManagement;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Users.Badges
{
    public class BadgeComponent
    {
        private readonly Habbo _player;
        private readonly Dictionary<string, Badge> _badges;

        public BadgeComponent(Habbo Player, UserData data)
        {
            _player = Player;
            _badges = new Dictionary<string, Badge>();

            foreach (Badge badge in data.badges)
            {

                if (!_badges.ContainsKey(badge.Code))
                {
                    _badges.Add(badge.Code, badge);
                }
            }
        }

        public int Count => _badges.Count;

        public DataTable GetBadgesOfflineUser(int UserId)
        {
            DataTable badges = null;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_badges` where badge_slot > 0 and user_id = @userid ORDER BY badge_slot ASC");
                dbClient.AddParameter("userid", UserId);

                badges = dbClient.GetTable();
            }

            return badges;
        }

        public int EquippedCount
        {
            get
            {
                int i = 0;

                foreach (Badge Badge in _badges.Values)
                {
                    if (Badge.Slot <= 0)
                    {
                        continue;
                    }

                    i++;
                }

                return i;
            }
        }

        public ICollection<Badge> GetBadges()
        {
            return _badges.Values;
        }

        public Badge GetBadge(string Badge)
        {
            if (_badges.ContainsKey(Badge))
            {
                return _badges[Badge];
            }

            return null;
        }

        public bool TryGetBadge(string BadgeCode, out Badge Badge)
        {
            return _badges.TryGetValue(BadgeCode, out Badge);
        }

        public bool HasBadge(string Badge)
        {
            return _badges.ContainsKey(Badge);
        }

        public bool HasBadgeList(List<string> Badge)
        {
            foreach (string str in Badge)
            {
                if (!_badges.ContainsKey(str))
                {
                    return false;
                }
            }
            return true;
        }

        public void GiveBadge(string Badge, Boolean InDatabase, GameClient Session)
        {
            if (HasBadge(Badge))
            {
                return;
            }

            if (InDatabase)
            {
                using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO user_badges (user_id,badge_id,badge_slot) VALUES (" + _player.Id + ",@badge," + 0 + ")");
                    dbClient.AddParameter("badge", Badge);
                    dbClient.RunQuery();
                }
            }

            _badges.Add(Badge, new Badge(Badge, 0));

            if (Session != null)
            {
                Session.SendMessage(new BadgesComposer(Session));
                Session.SendMessage(new FurniListNotificationComposer(1, 4));
            }
        }

        public void ResetSlots()
        {
            foreach (Badge Badge in _badges.Values)
            {
                Badge.Slot = 0;
            }
        }

        public void RemoveBadge(string Badge, GameClient Session)
        {
            if (!HasBadge(Badge))
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + _player.Id + " LIMIT 1");
                dbClient.AddParameter("badge", Badge);
                dbClient.RunQuery();
            }

            if (_badges.ContainsKey(Badge))
            {
                _badges.Remove(Badge);
            }
        }

        public void RemoveBadge(string Badge)
        {
            if (!HasBadge(Badge))
            {
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM user_badges WHERE badge_id = @badge AND user_id = " + _player.Id + " LIMIT 1");
                dbClient.AddParameter("badge", Badge);
                dbClient.RunQuery();
            }

            if (_badges.ContainsKey(Badge))
            {
                _badges.Remove(Badge);
            }
        }
    }
}