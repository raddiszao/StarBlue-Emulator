﻿using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using System;
using System.Data;
using System.Text;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class UserInfoCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[USUÁRIO]";
        public string Description => "Ver informações de um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário que você deseja ver e revisar informações.");
                return;
            }

            DataRow UserData = null;
            DataRow UserInfo = null;
            string Username = Params[1];

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`username`,`mail`,`rank`,`motto`,'look',`credits`,`activity_points`,`vip_points`,`guia`,`puntos_eventos`,`online`,`rank_vip` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.GetRow();
            }

            if (UserData == null)
            {
                Session.SendNotification("Não há usuário com esse nome " + Username + ".");
                return;
            }

            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + Convert.ToInt32(UserData["id"]) + "' LIMIT 1");
                UserInfo = dbClient.GetRow();
                if (UserInfo == null)
                {
                    dbClient.RunFastQuery("INSERT INTO `user_info` (`user_id`) VALUES ('" + Convert.ToInt32(UserData["id"]) + "')");

                    dbClient.SetQuery("SELECT * FROM `user_info` WHERE `user_id` = '" + Convert.ToInt32(UserData["id"]) + "' LIMIT 1");
                    UserInfo = dbClient.GetRow();
                }
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Username);

            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Convert.ToDouble(UserInfo["trading_locked"]));

            DateTime valecrack = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            valecrack = valecrack.AddSeconds(Session.GetHabbo().LastOnline).ToLocalTime();

            string time = valecrack.ToString();

            StringBuilder HabboInfo = new StringBuilder();//
            HabboInfo.Append("<font color='#0489B1'><b>Informação Geral:</b></font>\r");
            HabboInfo.Append("<font size='10'>ID: " + Convert.ToInt32(UserData["id"]) + "\r");
            HabboInfo.Append("Rank: " + Convert.ToInt32(UserData["rank"]) + "\r");
            HabboInfo.Append("VIP: " + Convert.ToInt32(UserData["rank_vip"]) + "\r");
            HabboInfo.Append("Email: " + Convert.ToString(UserData["mail"]) + "\r");
            HabboInfo.Append("Online: " + (TargetClient != null ? "Sim" : "Não") + "\r");
            HabboInfo.Append("Última conexão: " + time + "</font>\r\r");

            HabboInfo.Append("<font color='#0489B1'><b>Informação Monetária:</b></font>\r");
            HabboInfo.Append("<font size ='10'><font color='#F7D358'><b>Créditos:</b></font> " + Convert.ToInt32(UserData["credits"]) + "\r");
            HabboInfo.Append("<font color='#BF00FF'><b>Duckets:</b></font> " + Convert.ToInt32(UserData["activity_points"]) + "\r");
            HabboInfo.Append("<font color='#2E9AFE'><b>Diamantes:</b></font> " + Convert.ToInt32(UserData["vip_points"]) + "\r");
            HabboInfo.Append("<font color='#FE9A2E'><b>Pontos em eventos:</b></font> " + Convert.ToInt32(UserData["puntos_eventos"]) + "</font>\r\r");

            HabboInfo.Append("<font color='#0489B1'><b>Informação Moderação:</b></font>\r");
            HabboInfo.Append("<font size='10'>Bans: " + Convert.ToInt32(UserInfo["bans"]) + "\r");
            HabboInfo.Append("Reportes Enviados: " + Convert.ToInt32(UserInfo["cfhs"]) + "\r");
            HabboInfo.Append("Abuso: " + Convert.ToInt32(UserInfo["cfhs_abusive"]) + "\r");
            HabboInfo.Append("Trocas bloqueadas: " + (Convert.ToInt32(UserInfo["trading_locked"]) == 0 ? "Nenhum." : "Expira: " + (origin.ToString("dd/MM/yyyy")) + "") + "\r");
            HabboInfo.Append("Total trocas bloqueios: " + Convert.ToInt32(UserInfo["trading_locks_count"]) + "</font>\r\r");

            if (TargetClient != null)
            {
                HabboInfo.Append("<font color = '#0489B1'><b> Localização:</b></font>\r");
                if (!TargetClient.GetHabbo().InRoom)
                {
                    HabboInfo.Append("Não está em nenhum quarto.\r");
                }
                else
                {
                    HabboInfo.Append("<font size='10'>Quarto: " + TargetClient.GetHabbo().CurrentRoom.Name + " (" + TargetClient.GetHabbo().CurrentRoom.RoomId + ")\r");
                    HabboInfo.Append("Dono: " + TargetClient.GetHabbo().CurrentRoom.OwnerName + "\r");
                    HabboInfo.Append("Visitantes: " + TargetClient.GetHabbo().CurrentRoom.UserCount + " de " + TargetClient.GetHabbo().CurrentRoom.UsersMax);
                }
            }
            Session.SendMessage(new RoomNotificationComposer("Informação de " + Username + ":", (HabboInfo.ToString()), "usr/body/" + Username + "", "", ""));
        }
    }
}
