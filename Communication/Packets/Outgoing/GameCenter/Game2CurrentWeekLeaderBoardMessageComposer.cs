namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    public class Game2CurrentWeekLeaderboardMessageComposer : MessageComposer
    {
        public Game2CurrentWeekLeaderboardMessageComposer(/*GameData Game, int Week*/)
            : base(Composers.Game2CurrentWeekLeaderboardComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(1);
            packet.WriteInteger(10000);
            packet.WriteInteger(1);

            packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            packet.WriteString("m");//Gender .ToLower()  
            packet.WriteString("Custom");//Username
            // packet.WriteInteger(2018);
            // packet.WriteInteger(1);
            // packet.WriteInteger(0);
            // packet.WriteInteger(1);
            // packet.WriteInteger(1581);


            // packet.WriteInteger(Game.LeaderBoard.Count());//Count
            // int id = 0;
            // foreach(var Data in Game.LeaderBoard)
            // {
            //     id++;
            //     Habbo habbo = StarBlueServer.GetHabboById(Data.Value.UserId);
            //     packet.WriteInteger(habbo.Id);//Id
            //     packet.WriteInteger(id);//Rank
            //     packet.WriteInteger(Data.Value.Points);//Score
            //     packet.WriteString(habbo.Username);//Username
            //     packet.WriteString(habbo.Look);//Figure
            //     packet.WriteString(habbo.Gender.ToLower());//Gender .ToLower()

            //     if(id == 10) { break; }
            // }
            ///* packet.WriteInteger(1);//Id
            // packet.WriteInteger(10);//Rank
            // packet.WriteInteger(1);//Score
            // packet.WriteString("Custom - Derecha");//Username
            // packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(2);//Id
            // packet.WriteInteger(19999);//Score
            // packet.WriteInteger(2);//Rank
            // packet.WriteString("Salinas");//Username
            // packet.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(3);//Id
            // packet.WriteInteger(1232);//Score
            // packet.WriteInteger(3);//Rank
            // packet.WriteString("HiddenKey");//Username
            // packet.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(4);//Id
            // packet.WriteInteger(1000);//Score
            // packet.WriteInteger(4);//Rank
            // packet.WriteString("Custom");//Username
            // packet.WriteString("fa-1201-62.sh-6102459-96-62.hr-831-1031.ch-804-1201.lg-281-110.ha-1012-78.hd-180-11");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(5);//Id
            // packet.WriteInteger(1000);//Score
            // packet.WriteInteger(5);//Rank
            // packet.WriteString("Custom");//Username
            // packet.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(6);//Id
            // packet.WriteInteger(19999);//Score
            // packet.WriteInteger(6);//Rank
            // packet.WriteString("Salinas");//Username
            // packet.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(7);//Id
            // packet.WriteInteger(1232);//Score
            // packet.WriteInteger(7);//Rank
            // packet.WriteString("HiddenKey");//Username
            // packet.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(8);//Id
            // packet.WriteInteger(1000);//Score
            // packet.WriteInteger(8);//Rank
            // packet.WriteString("Custom");//Username
            // packet.WriteString("fa-1201-62.sh-6102459-96-62.hr-831-1031.ch-804-1201.lg-281-110.ha-1012-78.hd-180-11");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(9);//Id
            // packet.WriteInteger(1000);//Score
            // packet.WriteInteger(9);//Rank
            // packet.WriteString("Custom");//Username
            // packet.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            // packet.WriteString("m");//Gender .ToLower()

            // packet.WriteInteger(10);//Id
            // packet.WriteInteger(1000);//Score
            // packet.WriteInteger(10);//Rank
            // packet.WriteString("Custom");//Username
            // packet.WriteString("hd-180-11.hr-828-55.ch-804-96.sh-3089-1186.lg-281-110");//Figure
            // packet.WriteString("m");//Gender .ToLower()*/

            // packet.WriteInteger(1);//Unknown
            // packet.WriteInteger(Game.GameId);//Game Id?
        }
    }
}
