using StarBlue.Communication.Packets.Outgoing.GameCenter;
using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Games;
using System;
using System.Globalization;

namespace StarBlue.Communication.Packets.Incoming.GameCenter
{
    class GetGameCenterLeaderboardsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GameId = Packet.PopInt();
            int UserId = Packet.PopInt();
            int weekNum = new GregorianCalendar(GregorianCalendarTypes.Localized).GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int lastWeekNum = 0;

            if (weekNum == 1) { lastWeekNum = 52; } else { lastWeekNum = weekNum - 1; }


            if (StarBlueServer.GetGame().GetGameDataManager().TryGetGame(GameId, out GameData GameData))
            {
                Session.SendMessage(new Game2LastWeekLeaderboardMessageComposer(GameId, lastWeekNum)); // Derecha Whats funcionando
                Session.SendMessage(new Game2WeeklyLeaderboardComposer(GameId, weekNum)); // Funcionando Whats Izquierda
            }
        }
    }
}
