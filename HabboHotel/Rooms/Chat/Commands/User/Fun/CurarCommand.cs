using StarBlue.HabboHotel.Rooms.Games.Teams;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class CurarCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "";

        public string Description => "Curar um inchaço devido a um golpe recebido.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (ThisUser == null)
            {
                return;
            }

            if (ThisUser.RidingHorse)
            {
                Session.SendWhisper("Não pode ser curado de uma só vez enquanto você está no cavalo.", 34);
                return;
            }
            else if (ThisUser.Team != TEAM.NONE)
            {
                return;
            }
            else if (ThisUser.isLying)
            {
                return;
            }

            Session.GetHabbo().Effects().ApplyEffect(0);
            Session.SendWhisper("Você curou corretamente de um golpe no rosto.", 34);
        }
    }
}
