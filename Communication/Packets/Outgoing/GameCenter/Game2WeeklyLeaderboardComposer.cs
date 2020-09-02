﻿using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.Users;
using System;
using System.Data;

namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    public class Game2WeeklyLeaderboardComposer : ServerPacket // IZQUIERDA
    {
        public Game2WeeklyLeaderboardComposer(int GameId, int Week)
            : base(ServerPacketHeader.Game2WeeklyLeaderboardMessageComposer)
        {
            base.WriteInteger(2018);
            base.WriteInteger(1);
            base.WriteInteger(0);
            base.WriteInteger(1);
            base.WriteInteger(1581);

            int count = 0;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `games_leaderboard` WHERE game_id = " + GameId + " AND week = " + Week + " LIMIT 10");
                count = dbClient.GetInteger();
            }

            base.WriteInteger(count);//Count

            int id = 1;
            using (IQueryAdapter dbClient2 = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {

                DataTable GetLeaderData = null;
                dbClient2.SetQuery("SELECT * FROM `games_leaderboard` WHERE game_id = " + GameId + " AND week = " + Week + " LIMIT 10");
                GetLeaderData = dbClient2.GetTable();

                if (GetLeaderData != null)
                {
                    foreach (DataRow Rows in GetLeaderData.Rows)
                    {
                        Habbo habbo = StarBlueServer.GetHabboById(Convert.ToInt32(Rows["user_id"]));

                        base.WriteInteger(habbo.Id);//Id
                        base.WriteInteger(Convert.ToInt32(Rows["points"]));//Score
                        base.WriteInteger(id++);//Rank
                        base.WriteString(habbo.Username);//Username
                        base.WriteString(habbo.Look);//Figure       
                        base.WriteString(habbo.Gender.ToLower());//Gender .ToLower()
                    }
                }
            }

            base.WriteInteger(0);//
            base.WriteInteger(GameId);//Game Id?
            /* foreach (var Data in Game.LeaderBoard)
             {
                 if (Data.Value.Week != Week || Data.Value.GameId != Game.GameId) { return; id--; }
                 id++;
                
                 base.WriteInteger(habbo.Id);//Id
                 base.WriteInteger(id);//Rank
                 base.WriteInteger(Data.Value.Points);//Score
                 base.WriteString(habbo.Username);//Username
                 base.WriteString(habbo.Look);//Figure
                 base.WriteString(habbo.Gender.ToLower());//Gender .ToLower()

                 if (id == count) { break; }
             }*/

            //Used to generate the ranking numbers.
            //int num = 0;

            //base.WriteInteger(Habbos.Count);//Count
            //foreach (Habbo Habbo in Habbos.ToList())
            //{
            //    num++;
            //    base.WriteInteger(Habbo.Id);//Id
            //    base.WriteInteger(Habbo.FastfoodScore);//Score
            //    base.WriteInteger(num);//Rank
            //   base.WriteString(Habbo.Username);//Username
            //   base.WriteString(Habbo.Look);//Figure
            //   base.WriteString(Habbo.Gender.ToLower());//Gender .ToLower()
            //}

            //base.WriteInteger(0);//
            //base.WriteInteger(GameData.GameId);//Game Id?

            /*base.WriteInteger(10);//Count
            
            base.WriteInteger(1);//Id
            base.WriteInteger(10);//Rank
            base.WriteInteger(1);//Score
            base.WriteString("Custom - Izquierda");//Username
            base.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(2);//Id
            base.WriteInteger(19999);//Score
            base.WriteInteger(2);//Rank
            base.WriteString("Salinas");//Username
            base.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(3);//Id
            base.WriteInteger(1232);//Score
            base.WriteInteger(3);//Rank
            base.WriteString("HiddenKey");//Username
            base.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(4);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(4);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("fa-1201-62.sh-6102459-96-62.hr-831-1031.ch-804-1201.lg-281-110.ha-1012-78.hd-180-11");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(5);//Id
            base.WriteInteger(1000);//Score
            base.WriteInteger(5);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(6);//Id
            base.WriteInteger(10000);//Score
            base.WriteInteger(6);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(7);//Id
            base.WriteInteger(10000);//Score
            base.WriteInteger(7);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(8);//Id
            base.WriteInteger(10000);//Score
            base.WriteInteger(8);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(9);//Id
            base.WriteInteger(10000);//Score
            base.WriteInteger(9);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            base.WriteString("m");//Gender .ToLower()

            base.WriteInteger(10);//Id
            base.WriteInteger(10000);//Score
            base.WriteInteger(10);//Rank
            base.WriteString("Custom");//Username
            base.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            base.WriteString("m");//Gender .ToLower()*/
        }
    }
}
