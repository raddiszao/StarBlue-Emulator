
using StarBlue.Communication.Packets.Outgoing.GameCenter;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Games;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    class GetWeeklyLeaderBoardEvent : IPacketEvent // Get2GameWeeklySmallLeaderboardComposer
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();


            if (StarBlueServer.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData GameData))
            {
                Session.SendMessage(new Game2WeeklySmallLeaderboardComposer(GameId)); // El pequeño antes de que pulses nada. UNICO NECESARIO AQUI.
                Session.SendMessage(new GameCenterPrizeMessageComposer(GameId));
                Session.SendMessage(new GameCenterLuckyLoosersWinnersComposer(GameId));
                //Session.SendMessage(new Game2CurrentWeekLeaderboardMessageComposer(GameData, weekNum)); // Izquierda
                //Session.SendMessage(new Game2LastWeekLeaderboardMessageComposer(GameData, weekNum)); // Derecha
                //Session.SendMessage(new Game2WeeklyLeaderboardComposer(GameId)); sin custom
            }
        }
    }
}
