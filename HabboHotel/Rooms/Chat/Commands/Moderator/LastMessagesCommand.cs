using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.Database.Interfaces;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Data;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class LastMessagesCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[USUARIO]";
        public string Description => "Verifique as últimas mensagens do usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário que você deseja ver as informações.", 34);
                return;
            }

            DataRow UserData = null;
            string Username = Params[1];

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.GetRow();
            }

            if (UserData == null)
            {
                Session.SendNotification("Não existe ninguém com o nome " + Username + ".");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                Session.SendWhisper("O usuário precisa estar online para ver o histórico.", 34);
                return;
            }

            DataTable GetLogs = null;
            StringBuilder HabboInfo = new StringBuilder();

            HabboInfo.Append("Estas são as últimas mensagens do usuário suspeito, lembre - se de sempre rever estes casos antes de proceder à proibição, a menos que seja um caso óbvio de spam.\n\n");

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs` WHERE `user_id` = '" + TargetClient.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 10");
                GetLogs = dbClient.GetTable();

                if (GetLogs != null)
                {
                    int Number = 11;
                    foreach (DataRow Log in GetLogs.Rows)
                    {
                        Number -= 1;
                        HabboInfo.Append("<font size ='8' color='#B40404'><b>[" + Number + "]</b></font>" + " " + Convert.ToString(Log["message"]) + "\r");
                    }
                }
                else
                {
                    Session.SendMessage(new RoomCustomizedAlertComposer("Infelizmente o usuário que você solicitou não tem mensagens no registro."));
                }

                Session.SendMessage(new RoomNotificationComposer("Últimas mensagens de " + Username + ":", (HabboInfo.ToString()), "usr/body/" + Username + "", "", ""));



            }
        }
    }
}

