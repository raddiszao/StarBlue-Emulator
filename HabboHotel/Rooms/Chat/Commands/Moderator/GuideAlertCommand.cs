using StarBlue.Communication.Packets.Outgoing.Notifications;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GuideAlertCommand : IChatCommand
    {
        public string PermissionRequired => "command_guide_alert";

        public string Parameters => "%message%";

        public string Description => "Envia um alerta a todos os staffs online.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo()._guidelevel < 1)
            {
                Session.SendWhisper("Você não pode enviar alertas para guias se não estiver.");
                return;

            }
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva a mensagem que você deseja enviar.");
                return;
            }

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(StarBlueServer.GetUnixTimestamp()).ToLocalTime();

            string Message = CommandManager.MergeParams(Params, 1);
            StarBlueServer.GetGame().GetClientManager().GuideAlert(new MOTDNotificationComposer("[GUIDE]\r[" + dtDateTime + "]\r\r" + Message + "\r\r - " + Session.GetHabbo().Username + " [" + Session.GetHabbo()._guidelevel + "]"));
            return;
        }
    }
}