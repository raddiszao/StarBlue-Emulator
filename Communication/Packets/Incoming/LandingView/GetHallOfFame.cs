using Database_Manager.Database.Session_Details.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace StarBlue.Communication.Packets.Incoming.LandingView
{
    public class GetHallOfFame
    {
        private static List<UserCompetition> usersHof = new List<UserCompetition>();

        public static void Load()
        {
            if (usersHof.Count > 0)
            {
                usersHof.Clear();
            }

            using (IQueryAdapter dbQuery = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                usersHof = new List<UserCompetition>();

                dbQuery.SetQuery("SELECT * FROM `users` WHERE `rank` < '8' ORDER BY `puntos_eventos` DESC LIMIT 16");
                DataTable gUsersTable = dbQuery.GetTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    var user = new UserCompetition(Row);
                    if (!usersHof.Contains(user))
                    {
                        usersHof.Add(user);
                    }
                }
            }

        }

        public static List<UserCompetition> getHofUsers()
        {
            return usersHof;
        }
    }

    public class UserCompetition
    {
        public int userId, Rank, Score;
        public string userName, Look;

        public UserCompetition(DataRow row)
        {
            userId = (int)row["id"];
            userName = (string)row["username"];
            Look = (string)row["look"];
            Rank = Convert.ToInt32(row["rank"].ToString());
            Score = Convert.ToInt32(row["puntos_eventos"].ToString());
        }
    }
}
