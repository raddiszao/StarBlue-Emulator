using log4net;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Cache.Process;
using StarBlue.HabboHotel.GameClients;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Cache
{
    public class CacheManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Cache.CacheManager");
        private ConcurrentDictionary<int, UserCache> _usersCached;
        private ProcessComponent _process;

        public CacheManager()
        {
            _usersCached = new ConcurrentDictionary<int, UserCache>();
            _process = new ProcessComponent();
            _process.Init();
            log.Info(">> Cache Manager -> READY!");
        }
        public bool ContainsUser(int Id)
        {
            return _usersCached.ContainsKey(Id);
        }

        public UserCache GenerateUser(int Id)
        {
            UserCache User = null;

            if (_usersCached.ContainsKey(Id))
            {
                if (TryGetUser(Id, out User))
                {
                    return User;
                }
            }

            GameClient Client = StarBlueServer.GetGame().GetClientManager().GetClientByUserID(Id);
            if (Client != null)
            {
                if (Client.GetHabbo() != null)
                {
                    User = new UserCache(Id, Client.GetHabbo().Username, Client.GetHabbo().Motto, Client.GetHabbo().Look);
                    _usersCached.TryAdd(Id, User);
                    return User;
                }
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username`, `motto`, `look` FROM users WHERE id = @id LIMIT 1");
                dbClient.AddParameter("id", Id);

                DataRow dRow = dbClient.GetRow();

                if (dRow != null)
                {
                    User = new UserCache(Id, dRow["username"].ToString(), dRow["motto"].ToString(), dRow["look"].ToString());
                    _usersCached.TryAdd(Id, User);
                }

                dRow = null;
            }

            return User;
        }

        public bool TryRemoveUser(int Id, out UserCache User)
        {
            return _usersCached.TryRemove(Id, out User);
        }

        public bool TryGetUser(int Id, out UserCache User)
        {
            return _usersCached.TryGetValue(Id, out User);
        }

        public ICollection<UserCache> GetUserCache()
        {
            return _usersCached.Values;
        }

        public void Dispose()
        {
            _usersCached.Clear();
            _process.Dispose();
        }
    }
}