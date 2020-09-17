namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    public class Game2WeeklySmallLeaderboardComposer : MessageComposer
    {
        private int GameId { get; }

        public Game2WeeklySmallLeaderboardComposer(int GameId/*, ICollection<Habbo> Habbos*/)
            : base(Composers.Game2WeeklySmallLeaderboardComposer)
        {
            this.GameId = GameId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(2014);
            packet.WriteInteger(41);
            packet.WriteInteger(0);
            packet.WriteInteger(1);
            packet.WriteInteger(1581);

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

            packet.WriteInteger(3);//Count
            packet.WriteInteger(1);//Id
            packet.WriteInteger(1000);//Score
            packet.WriteInteger(1);//Rank
            packet.WriteString("X");//Username
            packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            packet.WriteString("m");//Gender .ToLower()

            packet.WriteInteger(1);//Id
            packet.WriteInteger(1000);//Score
            packet.WriteInteger(2);//Rank
            packet.WriteString("Salinas");//Username
            packet.WriteString("ch-255-96.sh-3115-1408-1408.lg-3116-85-1408.ea-1404-1194.fa-1203-1189.hr-831-1041.hd-3103-1389");//Figure
            packet.WriteString("m");//Gender .ToLower()

            packet.WriteInteger(1);//Id
            packet.WriteInteger(1000);//Score
            packet.WriteInteger(3);//Rank
            packet.WriteString("HiddenKey");//Username
            packet.WriteString("ch-235-1408.fa-1208-1189.lg-3116-85-1408.cc-886-62.ea-1404-1194.ha-3086-96-1194.sh-3115-1408-1408.hr-100-1041.hd-3103-1389");//Figure
            packet.WriteString("m");//Gender .ToLower()


            packet.WriteInteger(1);//
            packet.WriteInteger(GameId);//Game Id?
        }
    }
}
