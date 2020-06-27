using StarBlue.HabboHotel.GameClients;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class RemoveBadgeCommand : IChatCommand
    {
        public string PermissionRequired => "user_13";
        public string Parameters => "[USUARIO] [CODIGO]";
        public string Description => "Se utiliza para tirar o emblema de um usuário.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 3)
            {
                GameClient TargetClient = null;
                TargetClient = StarBlueServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
                if (TargetClient != null)
                {
                    if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                    {
                        {
                            Session.SendNotification("Este usuário não possui o emblema " + Params[2] + "");
                        }
                    }
                    else
                    {
                        RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                        TargetClient.GetHabbo().GetBadgeComponent().RemoveBadge(Params[2], TargetClient);
                        TargetClient.SendNotification("Seu emblema " + Params[2] + " foi removido por " + ThisUser.GetUsername() + "!");
                        Session.SendNotification("Emblema removido");

                    }
                }
            }
            else
            {
                Session.SendNotification("Usuario não encontrado.");
                return;
            }
        }
    }
}