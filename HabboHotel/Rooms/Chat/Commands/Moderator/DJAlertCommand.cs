using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class DJAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[USUARIO]";
        public string Description => "Enviar alerta para hotel de transmissão.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva a mensagem que você quer enviar");
                return;
            }
            string Message = CommandManager.MergeParams(Params, 1);
            StarBlueServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("DJAlertNEW", "DJ " + Message + " está transmitindo ao vivo! Sintonize " + Convert.ToString(StarBlueServer.GetConfig().data["hotel.name"]) + "FM agora mesmo e disfrute ao máximo.", ""));
            return;
        }
    }
}
