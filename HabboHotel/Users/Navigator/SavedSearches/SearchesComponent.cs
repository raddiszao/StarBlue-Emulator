using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;


namespace StarBlue.HabboHotel.Users.Navigator.SavedSearches
{
    public class SearchesComponent
    {
        private ConcurrentDictionary<int, SavedSearch> _savedSearches;

        public SearchesComponent()
        {
            _savedSearches = new ConcurrentDictionary<int, SavedSearch>();
        }

        public bool Init(Habbo Player)
        {
            if (_savedSearches.Count > 0)
            {
                _savedSearches.Clear();
            }

            DataTable GetSearches = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`filter`,`search_code` FROM `user_saved_searches` WHERE `user_id` = @UserId");
                dbClient.AddParameter("UserId", Player.Id);
                GetSearches = dbClient.GetTable();

                if (GetSearches != null)
                {
                    foreach (DataRow Row in GetSearches.Rows)
                    {
                        _savedSearches.TryAdd(Convert.ToInt32(Row["id"]), new SavedSearch(Convert.ToInt32(Row["id"]), Convert.ToString(Row["filter"]), Convert.ToString(Row["search_code"])));
                    }
                }
            }
            return true;
        }

        public ICollection<SavedSearch> Searches => _savedSearches.Values;

        public bool TryAdd(int Id, SavedSearch Search)
        {
            return _savedSearches.TryAdd(Id, Search);
        }

        public bool TryRemove(int Id, out SavedSearch Removed)
        {
            return _savedSearches.TryRemove(Id, out Removed);
        }
    }
}
