using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class KickCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";
        public string Parameters => "[USUARIO] [MENSAGEM]";
        public string Description => "Expulse o usuário e envie-lhe o motivo.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário.", 34);
                return;
            }

            if (Room == null || Room.RoomData.WhoCanKick != 2 && (Room.RoomData.WhoCanKick != 1 || !Room.CheckRights(Session, false, true)) && !Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Lamento, você não tem permissão para isso.", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente o usuário não existe ou não está online.", 34);
                return;
            }

            if (TargetClient.GetHabbo() == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente o usuário não existe ou não está online.", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Get a life.", 34);
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("O usuário não está no quarto", 34);
                return;
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(TargetClient.GetHabbo().CurrentRoomId, out Room TargetRoom))
            {
                return;
            }

            TargetRoom.GetRoomUserManager().RemoveUserFromRoom(TargetClient, true, true);
        }
    }
}
