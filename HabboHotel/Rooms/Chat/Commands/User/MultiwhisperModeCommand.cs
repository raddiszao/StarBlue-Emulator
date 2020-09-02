using StarBlue.HabboHotel.GameClients;
using StarBlue.HabboHotel.Users;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class MultiwhisperModeCommand : IChatCommand
    {
        public string PermissionRequired => "user_normal";

        public string Parameters => "[usuário]";

        public string Description => "Adiciona um usuário ao sussurro em grupo.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nome do usuário que deseja adicionar.", 34);
                return;
            }

            Habbo User = StarBlueServer.GetHabboByUsername(Params[1]);
            if (User == null || User.GetClient() == null)
            {
                Session.SendWhisper("Usuário não foi encontrado.", 34);
                return;
            }

            RoomUser RoomUser = Room.GetRoomUserManager().GetRoomUserByHabbo(User.Id);
            if (RoomUser == null)
            {
                Session.SendWhisper("Usuário não se encontra no quarto.", 34);
                return;
            }
            else if (User.CurrentRoomId != Session.GetHabbo().CurrentRoomId)
            {
                Session.SendWhisper("O usuário precisa estar no mesmo quarto que você.", 34);
                return;
            }

            if (!Room.MultiWhispers.Contains(RoomUser))
            {
                if (RoomUser.HabboId == Session.GetRoomUser().HabboId)
                {
                    Session.SendWhisper("Você não pode adicionar você mesmo ao grupo de sussurro.", 34);
                    return;
                }

                if (!Room.MultiWhispers.Contains(Session.GetRoomUser()))
                {
                    Room.MultiWhispers.Add(Session.GetRoomUser());
                }

                Room.MultiWhispers.Add(RoomUser);
                foreach (RoomUser User2 in Room.MultiWhispers)
                {
                    if (User2 == null || User2.GetClient() == null)
                    {
                        return;
                    }

                    User2.GetClient().SendWhisper("O usuário " + User.Username + " foi adicionado ao grupo de sussurro por " + Session.GetHabbo().Username + ". Para sussurrar no grupo use: Sussurrar group_whisper", 34);
                }
                User.GetClient().SendWhisper(Session.GetHabbo().Username + " te adicionou ao sussurro em grupo do quarto. Use: Sussurrar group_whisper para falar.", 34);
            }
            else
            {
                Session.SendWhisper("Este usuário já faz parte do grupo de sussurro.", 34);
            }
        }
    }
}