using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.Database.Interfaces;
using System;
using System.Data;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class StatsCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Mostra suas estatisticas.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            double Minutes = Session.GetHabbo().GetStats().OnlineTime / 60;
            double Hours = Minutes / 60;
            int OnlineTime = Convert.ToInt32(Hours);
            string s = OnlineTime == 1 ? "" : "s";

            DataRow UserInfo = null;

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                UserInfo = dbClient.GetRow();
            }

            StringBuilder List = new StringBuilder("");
            List.AppendLine("As informações da sua conta são:\n\n");

            List.AppendLine("Info Monetaria:");
            List.AppendLine("Creditos: " + Session.GetHabbo().Credits + "");
            List.AppendLine("Duckets: " + Session.GetHabbo().Duckets + "");
            List.AppendLine("Diamantes: " + Session.GetHabbo().Diamonds + "\n\n");

            List.AppendLine("Eventos:");
            List.AppendLine("Pontos em eventos: " + Convert.ToInt32(UserInfo["puntos_eventos"]) + "\n\n");

            List.AppendLine("Info personal:\n");
            List.AppendLine("Rank: " + Session.GetHabbo().Rank + "");
            List.AppendLine("Tempo online: " + OnlineTime + " Horas" + s + "");
            List.AppendLine("Respeitos: " + Session.GetHabbo().GetStats().Respect + "");


            Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
        }
    }
}
