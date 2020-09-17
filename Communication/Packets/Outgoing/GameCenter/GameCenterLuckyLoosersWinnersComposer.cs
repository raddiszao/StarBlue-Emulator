namespace StarBlue.Communication.Packets.Outgoing.GameCenter
{
    public class GameCenterLuckyLoosersWinnersComposer : MessageComposer
    {
        private int GameId { get; }

        public GameCenterLuckyLoosersWinnersComposer(int GameId)
            : base(Composers.GameCenterLuckyLoosersWinnersComposer)
        {
            this.GameId = GameId;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(1);
            packet.WriteString("PutoAmo");//Username
            packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            packet.WriteString("m");//Gender .ToLower()  
            packet.WriteInteger(1);//Rank       
            packet.WriteInteger(99999999);//Score

            //packet.WriteString("Custom");//Username
            //packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            //packet.WriteString("m");//Gender .ToLower()  
            //packet.WriteInteger(365);//Rank       
            //packet.WriteInteger(99999999);//Score

            //packet.WriteString("Custom");//Username
            //packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            //packet.WriteString("m");//Gender .ToLower()  
            //packet.WriteInteger(25);//Rank       
            //packet.WriteInteger(99999999);//Score

            //packet.WriteString("Custom");//Username
            //packet.WriteString("ch-235-1408.hd-3095-14.lg-3116-85-1408.sh-3115-1408-1408.ca-1805-64.ha-1002-1408");//Figure
            //packet.WriteString("m");//Gender .ToLower()  
            //packet.WriteInteger(14);//Rank       
            //packet.WriteInteger(99999999);//Score
        }
    }
}
