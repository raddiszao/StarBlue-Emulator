﻿using Database_Manager.Database.Session_Details.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace StarBlue.HabboHotel.Games
{
    public class GameDataManager
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.HabboHotel.Games.GameDataManager");

        private readonly Dictionary<int, GameData> _games;
        private readonly Dictionary<int, LeaderBoardData> _leaderBoardData;

        public GameDataManager()
        {
            _games = new Dictionary<int, GameData>();
            _leaderBoardData = new Dictionary<int, LeaderBoardData>();
            Init();
        }

        public void Init()
        {
            if (_games.Count > 0)
            {
                _games.Clear();
            }

            if (_leaderBoardData.Count > 0)
            {
                _leaderBoardData.Clear();
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetData = null;

                dbClient.SetQuery("SELECT `id`,`name`,`colour_one`,`colour_two`,`resource_path`,`string_three`,`game_swf`,`game_assets`,`game_server_host`,`game_server_port`,`socket_policy_port`,`game_enabled` FROM `games_config`");
                GetData = dbClient.GetTable();

                if (GetData != null)
                {
                    foreach (DataRow Row in GetData.Rows)
                    {
                        using (IQueryAdapter dbClient2 = StarBlueServer.GetDatabaseManager().GetQueryReactor())
                        {

                            DataTable GetLeaderData = null;
                            dbClient2.SetQuery("SELECT * FROM `games_leaderboard` WHERE game_id = " + Convert.ToInt32(Row["id"]) + " ORDER BY points ASC");
                            GetLeaderData = dbClient2.GetTable();

                            if (GetLeaderData != null)
                            {
                                foreach (DataRow Rows in GetLeaderData.Rows)
                                {
                                    LeaderBoardData value = new LeaderBoardData(Convert.ToInt32(Rows["game_id"]), Convert.ToInt32(Rows["user_id"]), Convert.ToInt32(Rows["points"]), Convert.ToInt32(Rows["record"]), Convert.ToInt32(Rows["week"]), Convert.ToInt32(Rows["year"]));
                                    _leaderBoardData.Add(Convert.ToInt32(Rows["id"]), value);
                                }
                            }
                            _games.Add(Convert.ToInt32(Row["id"]), new GameData(Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["colour_one"]), Convert.ToString(Row["colour_two"]), Convert.ToString(Row["resource_path"]), Convert.ToString(Row["string_three"]), Convert.ToString(Row["game_swf"]), Convert.ToString(Row["game_assets"]), Convert.ToString(Row["game_server_host"]), Convert.ToString(Row["game_server_port"]), Convert.ToString(Row["socket_policy_port"]), StarBlueServer.EnumToBool(Row["game_enabled"].ToString()), _leaderBoardData));

                        }
                    }
                }
            }

            log.Info(">> GameCenter Manager -> READY!");

        }

        public bool TryGetGame(int GameId, out GameData GameData)
        {
            if (_games.TryGetValue(GameId, out GameData))
            {
                return true;
            }

            return false;
        }

        public bool TryGetLeaderBoardData(int GameId, out LeaderBoardData LeaderBoard)
        {
            if (_leaderBoardData.TryGetValue(GameId, out LeaderBoard))
            {
                return true;
            }

            return false;
        }

        public int GetCount()
        {
            int GameCount = 0;
            foreach (GameData Game in _games.Values.ToList())
            {
                if (Game.GameEnabled)
                {
                    GameCount += 1;
                }
            }
            return GameCount;
        }

        public int GetLeaderCount()
        {
            int LeaderCount = 0;
            foreach (LeaderBoardData Game in _leaderBoardData.Values.ToList())
            {
                LeaderCount += 1;
            }
            return LeaderCount;
        }

        public ICollection<GameData> GameData => _games.Values;

        public ICollection<LeaderBoardData> LeaderData => _leaderBoardData.Values;
    }
}