using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class UserMessageCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[USUARIO] [MENSAGEM]";
        public string Description => "Enviar mensagem a um usuario.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome de usuário do usuário.", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro ao pesquisar pelo usuário, talvez eles não estejam online.", 34);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro ao pesquisar pelo usuário, talvez eles não estejam online.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Consiga uma vida.", 34);
                return;
            }

            string Message = CommandManager.MergeParams(Params, 2);

            TargetClient.SendMessage(new RoomNotificationComposer("command_gmessage", "message", "" + Message + "!"));
            Session.SendMessage(new RoomNotificationComposer("command_gmessage", "message", "Mensagem enviada com sucesso para " + TargetClient.GetHabbo().Username));
        }
    }
}
