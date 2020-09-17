using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Users;
using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    public class Game2LastWeekLeaderboardMessageComposer : MessageComposer
    {
        private int GameId { get; }
        private int Week { get; }

        public Game2LastWeekLeaderboardMessageComposer(int GameId, int Week)
            : base(Composers.Game2LastWeekLeaderboardMessageComposer)
        {
            this.GameId = GameId;
            this.Week = Week;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(2018);
            packet.WriteInteger(1);
            packet.WriteInteger(0);
            packet.WriteInteger(1);
            packet.WriteInteger(1581);

            int count = 0;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `games_leaderboard` WHERE game_id = " + GameId + " AND week = " + Week + " LIMIT 5");
                count = dbClient.GetInteger();
            }

            packet.WriteInteger(count);//Count

            int id = 1;
            using (IQueryAdapter dbClient2 = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {

                DataTable GetLeaderData = null;
                dbClient2.SetQuery("SELECT * FROM `games_leaderboard` WHERE game_id = " + GameId + " AND week = " + Week + " LIMIT 5");
                GetLeaderData = dbClient2.GetTable();

                if (GetLeaderData != null)
                {
                    foreach (DataRow Rows in GetLeaderData.Rows)
                    {
                        Habbo habbo = StarBlueServer.GetHabboById(Convert.ToInt32(Rows["user_id"]));

                        packet.WriteInteger(habbo.Id);//Id
                        packet.WriteInteger(Convert.ToInt32(Rows["points"]));//Score
                        packet.WriteInteger(id++);//Rank
                        packet.WriteString(habbo.Username);//Username
                        packet.WriteString(habbo.Look);//Figure       
                        packet.WriteString(habbo.Gender.ToLower());//Gender .ToLower()
                    }
                }
            }

            packet.WriteInteger(0);//
            packet.WriteInteger(GameId);//Game Id?
                                        //int count = 0;

            //if (Game.LeaderBoard.Count() > 5) { count = 5; } else { count = Game.LeaderBoard.Count(); }
            //packet.WriteInteger(2018);
            //packet.WriteInteger(1);
            //packet.WriteInteger(0);
            //packet.WriteInteger(1);
            //packet.WriteInteger(1581);

            //packet.WriteInteger(count);//Count
            //Console.WriteLine(Game.GameName + ":" + Game.LeaderBoard.Count());
            //int id = 0;
            //foreach (var Data in Game.LeaderBoard)
            //{
            //    if(Data.Value.Week != Week) { return; }
            //    id++;

            //    Habbo habbo = StarBlueServer.GetHabboById(Data.Value.UserId);
            //    packet.WriteInteger(habbo.Id);//Id
            //    packet.WriteInteger(id);//Rank
            //    packet.WriteInteger(Data.Value.Points);//Score
            //    packet.WriteString(habbo.Username);//Username
            //    packet.WriteString(habbo.Look);//Figure
            //    packet.WriteString(habbo.Gender.ToLower());//Gender .ToLower()

            //     if(id == 5) { break; }
            //}

            //Used to generate the ranking numbers.
            //int num = 0;

            //packet.WriteInteger(Habbos.Count);//Count
            //foreach (Habbo Habbo in Habbos.ToList())
            //{
            //    num++;
            //    packet.WriteInteger(Habbo.Id);//Id
            //    packet.WriteInteger(Habbo.FastfoodScore);//Score
            //    packet.WriteInteger(num);//Rank
            //   packet.WriteString(Habbo.Username);//Username
            //   packet.WriteString(Habbo.Look);//Figure
            //   packet.WriteString(Habbo.Gender.ToLower());//Gender .ToLower()
            //}

            //packet.WriteInteger(0);//
            //packet.WriteInteger(GameData.GameId);//Game Id?

            /*packet.WriteInteger(5);//Count

            packet.WriteInteger(1);//Id
            packet.WriteInteger(10);//Rank
            packet.WriteInteger(1);//Score
            packet.WriteString("Custom - Derecha");//Username
            packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            packet.WriteString("m");//Gender .ToLower()

            packet.WriteInteger(2);//Id
            packet.WriteInteger(19999);//Score
            packet.WriteInteger(2);//Rank
            packet.WriteString("Salinas");//Username
            packet.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            packet.WriteString("m");//Gender .ToLower()

            packet.WriteInteger(3);//Id
            packet.WriteInteger(1232);//Score
            packet.WriteInteger(3);//Rank
            packet.WriteString("HiddenKey");//Username
            packet.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            packet.WriteString("m");//Gender .ToLower()

            packet.WriteInteger(4);//Id
            packet.WriteInteger(1000);//Score
            packet.WriteInteger(4);//Rank
            packet.WriteString("Custom");//Username
            packet.WriteString("fa-1201-62.sh-6102459-96-62.hr-831-1031.ch-804-1201.lg-281-110.ha-1012-78.hd-180-11");//Figure
            packet.WriteString("m");//Gender .ToLower()

            packet.WriteInteger(5);//Id
            packet.WriteInteger(1000);//Score
            packet.WriteInteger(5);//Rank
            packet.WriteString("Custom");//Username
            packet.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            packet.WriteString("m");//Gender .ToLower()*/

            //packet.WriteInteger(0);//
            //packet.WriteInteger(Game.GameId);//Game Id?
        }
    }
}
