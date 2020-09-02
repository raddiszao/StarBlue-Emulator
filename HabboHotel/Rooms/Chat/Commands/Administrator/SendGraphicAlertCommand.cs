
using StarBlue.Communication.Packets.Outgoing.Notifications;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class SendGraphicAlertCommand : IChatCommand
    {
        public string PermissionRequired => "user_15";

        public string Parameters => "[IMAGEM]";

        public string Description => "Envie uma mensagem de alerta com imagem para todo o hotel.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, escreva o nome da imagem para enviar.");
                return;
            }

            StarBlueServer.GetGame().GetClientManager().SendMessage(new GraphicAlertComposer(Params[1]));
            return;
        }
    }
}
