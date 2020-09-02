using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class FreezeCommand : IChatCommand
    {
        public string PermissionRequired => "user_12";
        public string Parameters => "[USUARIO]";
        public string Description => "Congelar um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite o nome de quem você quer congelar.", 34);
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ocorreu um erro, aparentemente, o usuário não existe ou não está online", 34);
                return;
            }

            if (TargetClient.GetHabbo().Username == "Raddis")
            {
                Session.SendWhisper("Você não pode congelar esse usuário!", 34);
                return;
            }

            RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
            if (TargetUser != null)
            {
                TargetUser.Frozen = true;
            }

            Session.SendWhisper("Congelou corretamente " + TargetClient.GetHabbo().Username + "!");
        }
    }
}
