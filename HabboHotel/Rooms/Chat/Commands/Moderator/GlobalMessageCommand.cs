using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;
using System.Linq;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class GlobalMessageCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[MENSAGEM]";
        public string Description => "Enviar alerta 'BUBBLE' global";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {


            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduce el mensaje.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            foreach (GameClient client in StarBlueServer.GetGame().GetClientManager().GetClients.ToList())
            {
                client.SendMessage(new RoomNotificationComposer("command_gmessage", "message", "" + Message + "!"));
            }
        }
    }
}
