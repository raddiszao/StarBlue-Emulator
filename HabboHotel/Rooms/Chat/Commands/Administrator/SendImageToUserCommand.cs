
using StarBlue.Communication.Packets.Outgoing.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class SendImageToUserCommand : IChatCommand
    {
        public string PermissionRequired => "user_13";

        public string Parameters => "[USUARIO] [IMAGEM]";

        public string Description => "Enviar uma mensagem de aviso para um usuário";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome do usuário para quem você enviará o alerta.");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente o usuário não foi encontrado ou não está online.");
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente o usuário não foi encontrado ou não está online.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 2);

            TargetClient.SendMessage(new GraphicAlertComposer(Message));
            Session.SendWhisper("Alerta enviado com sucesso para " + TargetClient.GetHabbo().Username + ".");

        }
    }
}
