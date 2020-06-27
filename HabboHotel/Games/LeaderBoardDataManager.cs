using Database_Manager.Database.Session_Details.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.HabboHotel.Games
{
    public class LeaderBoardDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Games.GameDataManager");

        internal Dictionary<int, LeaderBoardData> _leaderboards;

        public LeaderBoardDataManager()
        {
            _leaderboards = new Dictionary<int, LeaderBoardData>();

            Init();
        }

        public void Init()
        {
            if (_leaderboards.Count > 0)
            {
                _leaderboards.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetData = null;
                dbClient.SetQuery("SELECT * FROM `games_leaderboard`");
                GetData = dbClient.GetTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        LeaderBoardData value = new LeaderBoardData(Convert.ToInt32(Row["game_id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["points"]), Convert.ToInt32(Row["record"]), Convert.ToInt32(Row["week"]), Convert.ToInt32(Row["year"]));
                        _leaderboards.Add(Convert.ToInt32(Row["id"]), value);
                    }
                }
            }

            log.Info(">> LeaderBoardData Manager -> READY!");
        }

        public bool TryGetLeaderBoardData(int GameId, out LeaderBoardData LeaderBoardData)
        {
            if (_leaderboards.TryGetValue(GameId, out LeaderBoardData))
            {
                return true;
            }

            return false;
        }

        public bool TryGetLeaderBoardDataWithWeek(int GameId, int Week, out LeaderBoardData LeaderBoardData)
        {
            if (_leaderboards.TryGetValue(Week, out LeaderBoardData) && _leaderboards.TryGetValue(GameId, out LeaderBoardData))
            {
                return true;
            }

            return false;
        }

        public ICollection<LeaderBoardData> LeaderBoardData
        {
            get
            {
                return _leaderboards.Values;
            }
        }

        public Dictionary<int, LeaderBoardData> getLeaderBoards()
        {
            return _leaderboards;
        }
    }
}
