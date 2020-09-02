using log4net;
using StarBlue.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.Core.Rank
{
    public class RankManager
    {
        private readonly Dictionary<int, RankData> _ranks;

        private static readonly ILog log = LogManager.GetLogger("StarBlue.Core.Rank.RankManager");

        public RankManager()
        {
            _ranks = new Dictionary<int, RankData>();
        }

        public void Init()
        {
            if (_ranks.Count > 0)
            {
                _ranks.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `ranks`");
                DataTable Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        _ranks.Add(Convert.ToInt32(Row["id"]), new RankData(Row["name"].ToString(), Row["badgeid"].ToString()));
                    }
                }
            }

            log.Info(">> Loaded " + _ranks.Count + " Ranks.");
        }

        public bool TryGetValue(int value, out RankData Rank)
        {
            if (_ranks.TryGetValue(value, out Rank))
            {
                return true;
            }

            return false;
        }

        public Dictionary<int, RankData> GetRanks()
        {
            return _ranks;
        }

        public class RankData
        {
            public string Name { get; set; }
            public string Badge { get; set; }

            public RankData(string Name, string Badge)
            {
                this.Name = Name;
                this.Badge = Badge;
            }
        }
    }
}
