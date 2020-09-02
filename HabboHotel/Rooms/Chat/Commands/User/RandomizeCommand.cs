using StarBlue.HabboHotel.GameClients;
using System;

namespace StarBlue.HabboHotel.Rooms.Chat.Commands.User
{
    internal class RandomizeCommand : IChatCommand
    {
        public string PermissionRequired => "user_vip";

        public string Parameters => "%min% %max%";

        public string Description => "Gera um numero aleatorio de 2 digitos.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            int.TryParse(Params[1], out int Rand1);
            int.TryParse(Params[2], out int Rand2);

            Random Rand = new Random();

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            User.OnChat(8, "Você pediu um numero aleatório entre " + Rand1 + " e " + Rand2 + " e obteve " + Rand.Next(Rand1, Rand2) + ".", false);

        }
    }
}
