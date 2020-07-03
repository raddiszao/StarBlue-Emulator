using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class DanceCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[ID]";

        public string Description => "Ativar uma dança em seu personagem, de 0 a 4.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser ThisUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, digite un ID de uma dança.", 34);
                return;
            }

            if (int.TryParse(Params[1], out int DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.SendWhisper("O ID da dança deve estar entre 0 e 4!", 34);
                    return;
                }

                Session.GetHabbo().CurrentRoom.SendMessage(new DanceComposer(ThisUser, DanceId));
            }
            else
            {
                Session.SendWhisper("Por favor, digite um número.", 34);
            }
        }
    }
}
