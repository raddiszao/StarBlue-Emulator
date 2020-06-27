using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class EmptyUserCommand : IChatCommand
    {
        public string PermissionRequired => "user_14";
        public string Parameters => "[USUARIO]";
        public string Description => "Limpa o inventario de um usuário";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva o nome de usuário que deseja limpar o inventario.");
                return;
            }

            GameClient TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Oops! Provavelmente o usuário não está online.");
                return;
            }

            if (TargetClient.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendWhisper("Você não pode limpar o inventario deste usuário");
                return;
            }

            TargetClient.GetHabbo().GetInventoryComponent().ClearItems();
        }
    }
}